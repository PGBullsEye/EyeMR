// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GestureDetectionServiceTests.cs"  company="PG BullsEye">
//   Author: Daniela Betzl
// </copyright>
// <summary>
//   This class contains unit tests for the GestureDetectionService
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;
using BullsEye.Scripts.Interfaces;
using BullsEye.Scripts.Model.Fixation;
using BullsEye.Scripts.Services;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// This class contains unit tests for the GestureDetectionService
/// </summary>
[TestFixture]
public class GestureDetectionServiceTests
{
    private GameObject gameObject;

    /// <summary>
    /// Setting up the objects for the tests and instantiate the GestureDetectionService.
    /// </summary>
    [TestFixtureSetUp]
    public void Init()
    {
        this.gameObject = new GameObject("ServiceProviderHolder");
        this.gameObject.AddComponent<ServiceProvider>();
        ServiceProvider.Instance.DrawGestures = false;
    }

    /// <summary>
    /// Cleaning up the objects after the tests.
    /// </summary>
    [TearDown]
    public void Dispose()
    {
        ServiceProvider.Get<FixationDetectionService>().Reset();
        ServiceProvider.Get<GestureDetectionService>().Reset();
    }

    /// <summary>
    /// The cleanup.
    /// </summary>
    [TestFixtureTearDown]
    public void Cleanup()
    {
        Object.DestroyImmediate(ServiceProvider.Instance);
        Object.DestroyImmediate(this.gameObject);
    }

    /// <summary>
    /// Unit Test Case for Start-method. 
    /// Condition: GestureDetectionService is instantiated.
    /// Result: GestureDetectionService is subscribed to FixationDetectionService correctly.
    /// </summary>
    [Test]
    public void StartIsSubscribed()
    {
        ServiceProvider.Get<GestureDetectionService>().Start();
        var listActual = ServiceProvider.Get<FixationDetectionService>().GetSubscribedTechniques();

        if (listActual == null)
        {
            Assert.Fail("Assert Fail: FixationDetectionService got none subscribed scripts, which should not happen.");
        }

        if (listActual.Count() != 1)
        {
            Assert.Fail("Assert Fail: FixationDetectionService got {0} subscribed scripts, which should not happen.", listActual.Count);
        }

        // check if both lists contains same key and values
        Assert.IsTrue(
            listActual.Any(f => f.InteractionTechnique.Equals(ServiceProvider.Get<GestureDetectionService>())),
            "Assert Fail: GestureDetectionService did not subscribed to FixationDetectionService correctly.");
        var serviceData = listActual.First(
            f => f.InteractionTechnique.Equals(ServiceProvider.Get<GestureDetectionService>()));
        Assert.IsTrue(
            serviceData.FixationTime.Equals(350),
            "Assert Fail: FixationTime is not correctly set, it's {0} instead 350.",
            serviceData.FixationTime);
        Assert.IsTrue(
            serviceData.Deviation.Equals(10),
            "Assert Fail: Scattering is not correctly set, it's {0} instead 10.",
            serviceData.Deviation);
        Assert.IsFalse(
            serviceData.CheckforGameObject,
            "Assert Fail: GameObject of GestureDetectionService is not null.");
    }

    /// <summary>
    /// Unit Test Case for Reset-method. 
    /// Condition: A techniques is subscribed to GestureDetectionService.
    /// Result: No technique is subscribed to GestureDetectionService.
    /// </summary>
    [Test]
    public void ResetIsResetted()
    {   
        // Add a technique
        this.gameObject.AddComponent<DummyTechnique>();
        this.gameObject.GetComponent<DummyTechnique>().Start();

        // And reset the Services
        ServiceProvider.Get<FixationDetectionService>().Reset();
        ServiceProvider.Get<GestureDetectionService>().Reset();

        var listActual = ServiceProvider.Get<GestureDetectionService>().GetSubscribedTechniques();

        Assert.IsTrue(
            listActual.Count == 0,
            "Assert Fail: GestureDetectionService got a subscribed technique, which should not happen.");
    }

    /// <summary>
    /// Unit Test Case for Subscribe-method. 
    /// Condition: No techniques is subscribed to GestureDetectionService.
    /// Result: One dummy technique subscribes successfully to GestureDetectionService.
    /// </summary>
    [Test]
    public void SubscribeSuccessful()
    {
        // create dummy technique and subscribe to GestureDetectionService
        this.gameObject.AddComponent<DummyTechnique>();
        this.gameObject.GetComponent<DummyTechnique>().Start();

        var gesture = this.gameObject.GetComponent<DummyTechnique>().GetGesture();
        var listActual = ServiceProvider.Get<GestureDetectionService>().GetSubscribedTechniques();
        
        if (listActual == null || listActual.Count() != 1)
        {
            Assert.Fail("Assert Fail: GestureDetectionService got none or more than 1 subscribed technique, which should not happen.");
        }

        // check if DummyTechnique subscribed correctly to the service and the list entry contains the correct values
        Assert.IsTrue(
            listActual.Any(g => g.InteractionTechnique.Equals(this.gameObject.GetComponent<DummyTechnique>())),
            "Assert Fail: GestureTechnique did not subscribed to GestureDetectionService correctly.");
        var techniqueData = listActual.First(
            g => g.InteractionTechnique.Equals(this.gameObject.GetComponent<DummyTechnique>()));
        Assert.IsTrue(
            techniqueData.Gesture.Angle[0].Equals(gesture.Angle[0]),
            "Assert Fail: Gesture is not correctly set, it's {0} instead {1}.",
            techniqueData.Gesture.Angle[0],
            gesture.Angle[0]);
        Assert.IsTrue(
            techniqueData.Gesture.Edges[0].Equals(gesture.Edges[0]),
            "Assert Fail: Gesture is not correctly set, it's {0} instead {1}.",
            techniqueData.Gesture.Edges[0],
            gesture.Edges[0]);
        Assert.IsTrue(
            techniqueData.ErrorAngle.Equals(30),
            "Assert Fail: ErrorAngle is not correctly set, it's {0} instead of 30.",
            techniqueData.ErrorAngle);
        Assert.IsTrue(
            techniqueData.ErrorDistance.Equals(0.5),
            "Assert Fail: ErrorDistance is not correctly set, it's {0} instead 0.2",
            techniqueData.ErrorDistance);
    }

    /// <summary>
    /// Unit Test Case for Subscribe-method. 
    /// Condition: No techniques is subscribed to GestureDetectionService.
    /// Result: Two dummy techniques subscribe successfully to GestureDetectionService.
    /// </summary>
    [Test]
    public void SubscribeTwoSuccessful()
    {
        // create dummy technique and subscribe to GestureDetectionService
        this.gameObject.AddComponent<DummyTechnique>();
        this.gameObject.GetComponent<DummyTechnique>().Start();
        this.gameObject.AddComponent<DummyTechnique2>();
        this.gameObject.GetComponent<DummyTechnique2>().Start();

        var gesture = this.gameObject.GetComponent<DummyTechnique>().GetGesture();
        var gesture2 = this.gameObject.GetComponent<DummyTechnique2>().GetGesture();
        var listActual = ServiceProvider.Get<GestureDetectionService>().GetSubscribedTechniques();

        if (listActual == null || listActual.Count() != 2)
        {
            Assert.Fail("Assert Fail: GestureDetectionService got none, one or more than 2 subscribed technique, which should not happen.");
        }

        // check if DummyTechniques subscribed correctly to the service and the list entry contains the correct values
        // Check gesture 1
        Assert.IsTrue(
            listActual.Any(g => g.InteractionTechnique.Equals(this.gameObject.GetComponent<DummyTechnique>())),
            "Assert Fail: GestureTechnique did not subscribed to GestureDetectionService correctly.");
        var techniqueData = listActual.First(
            g => g.InteractionTechnique.Equals(this.gameObject.GetComponent<DummyTechnique>()));
        Assert.IsTrue(
            techniqueData.Gesture.Angle[0].Equals(gesture.Angle[0]),
            "Assert Fail: Gesture is not correctly set, it's {0} instead {1}.",
            techniqueData.Gesture.Angle[0],
            gesture.Angle[0]);
        Assert.IsTrue(
            techniqueData.Gesture.Edges[0].Equals(gesture.Edges[0]),
            "Assert Fail: Gesture is not correctly set, it's {0} instead {1}.",
            techniqueData.Gesture.Edges[0],
            gesture.Edges[0]);
        Assert.IsTrue(
            techniqueData.ErrorAngle.Equals(30),
            "Assert Fail: ErrorAngle is not correctly set, it's {0} instead of 30.",
            techniqueData.ErrorAngle);
        Assert.IsTrue(
            techniqueData.ErrorDistance.Equals(0.5),
            "Assert Fail: ErrorDistance is not correctly set, it's {0} instead 0.2",
            techniqueData.ErrorDistance);

        // Check Gesture 2
        Assert.IsTrue(
            listActual.Any(g => g.InteractionTechnique.Equals(this.gameObject.GetComponent<DummyTechnique2>())),
            "Assert Fail: GestureTechnique did not subscribed to GestureDetectionService correctly.");
        var techniqueData2 = listActual.First(
            g => g.InteractionTechnique.Equals(this.gameObject.GetComponent<DummyTechnique2>()));
        Assert.IsTrue(
            techniqueData2.Gesture.Angle[0].Equals(gesture.Angle[0]),
            "Assert Fail: Gesture is not correctly set, it's {0} instead {1}.",
            techniqueData2.Gesture.Angle[0],
            gesture.Angle[0]);
        Assert.IsTrue(
            techniqueData2.Gesture.Edges[0].Equals(gesture.Edges[0]),
            "Assert Fail: Gesture is not correctly set, it's {0} instead {1}.",
            techniqueData2.Gesture.Edges[0],
            gesture.Edges[0]);
        Assert.IsTrue(
            techniqueData2.ErrorAngle.Equals(30),
            "Assert Fail: ErrorAngle is not correctly set, it's {0} instead of 30.",
            techniqueData2.ErrorAngle);
        Assert.IsTrue(
            techniqueData2.ErrorDistance.Equals(0.5),
            "Assert Fail: ErrorDistance is not correctly set, it's {0} instead 0.2",
            techniqueData2.ErrorDistance);
    }

    /// <summary>
    /// Unit Test Case for Subscribe-method. 
    /// Condition: A techniques is subscribed to GestureDetectionService and it tries to subscribe a second time.
    /// Result: The GestureDetectionService throws an ArgumentException.
    /// </summary>
    [Test]
    public void SubscribeNotSecondTime()
    {
        // create dummy technique and subscribe to GestureDetectionService
        this.gameObject.AddComponent<DummyTechnique>();
        this.gameObject.GetComponent<DummyTechnique>().Start();

        // try to subscribe a second time
        this.gameObject.GetComponent<DummyTechnique>().Subscribe();

        var subscribed = ServiceProvider.Get<GestureDetectionService>().GetSubscribedTechniques();

        if (subscribed == null)
        {
            Assert.Fail("Assert Fail: FixationDetectionService got none subscribed fixations, which should not happen.");
        }

        if (subscribed.Count != 1)
        {
            Assert.Fail("Assert Fail: FixationDetectionService got {0} instead of 1 subscribed fixations. The fixation technique could subscribe twice, which should not happen.", subscribed.Count);
        }
    }
    
    /// <summary>
     /// Unit Test Case for Unsubscribe-method. 
     /// Condition: One technique is subscribed to the GestureDetectionService.
     /// Result: The technique is successfully unsubscribed from the GestureDetectionService.
     /// </summary>
    [Test]
    public void UnsubscribeOnlyTechniqueSuccessful()
    {
        DummyTechnique dummy = this.gameObject.AddComponent<DummyTechnique>();
        dummy.Start();

        // Check first if it's subscribed
        var listActual = ServiceProvider.Get<GestureDetectionService>().GetSubscribedTechniques();

        if (listActual == null || listActual.Count() != 1)
        {
            Assert.Fail("Assert Fail: GestureDetectionService got none or more than 1 subscribed technique, which should not happen.");
        }

        // Then unsubscribe
        dummy.Unsubscribe();
        listActual = ServiceProvider.Get<GestureDetectionService>().GetSubscribedTechniques();

        if (listActual.Count() != 0)
        {
            Assert.Fail("Assert Fail: The gesture technique could not unsubscribe properly. Subscribed techniques: {0}", listActual.Count());
        }
    }

    /// <summary>
    /// Unit Test Case for Unsubscribe-method. 
    /// Condition: Two techniques are subscribed to the GestureDetectionService.
    /// Result: The first technique is successfully unsubscribed from the GestureDetectionService.
    /// </summary>
    [Test]
    public void UnsubscribeSuccessfulWithRemainingTechnique()
    {
        DummyTechnique dummy = this.gameObject.AddComponent<DummyTechnique>();
        dummy.Start();
        DummyTechnique2 dummy2 = this.gameObject.AddComponent<DummyTechnique2>();
        dummy2.Start();

        // Check first if they're subscribed
        var listActual = ServiceProvider.Get<GestureDetectionService>().GetSubscribedTechniques();

        if (listActual == null || listActual.Count() != 2)
        {
            Assert.Fail("Assert Fail: GestureDetectionService got none, less or more than 2 subscribed technique, which should not happen.");
        }

        // Then unsubscribe one technique
        dummy.Unsubscribe();
        listActual = ServiceProvider.Get<GestureDetectionService>().GetSubscribedTechniques();

        Assert.False(
            listActual.Any(g => g.InteractionTechnique.Equals(this.gameObject.GetComponent<DummyTechnique>())),
            "Assert Fail: The gesture technique could not unsubscribe properly. Subscribed techniques: {0}",
            listActual.Count);
    }

    /// <summary>
    /// Unit Test Case for OnFixStarted-method. 
    /// Condition: GestureDetectionService has no subscribed techniques and a fixation occurs.
    /// Result: GestureDetectionService should abort the fixation.
    /// </summary>
    [Test]
    public void OnFixStartedNoSubscribedTechniques()
    {
        // create a dummy fixation for testing
        var fixDummy = new Fixation(this.gameObject, null, new Vector2(5, 5));

        ServiceProvider.Get<GestureDetectionService>().GetSubscribedTechniques().Clear();
        this.SimulateFixation(fixDummy);

        // check if nothing changed
        Assert.IsFalse(
            ServiceProvider.Get<GestureDetectionService>().GetSubscribedTechniques().Any(),
            "Assert Fail: SubscribedTechniques is not empty which should be");
        Assert.IsTrue(
            ServiceProvider.Get<GestureDetectionService>().GetLastFixation().Equals(new Vector2(0, 0)),
            "Asser Fail: LastFixation is set though there is no subscribed techniques.");
    }

    /// <summary>
    /// Unit Test Case for OnFixStarted-method. 
    /// Condition: GestureDetectionService has one subscribed technique and a fixation occurs.
    /// Result: Fixation is recognized by the technique and is added to the observed fixations of the technique.
    /// </summary>
    [Test]
    public void OnFixStartedWithSubscribedTechniqueAndOneFixation()
    {
        // create a dummy fixation for testing
        var fixDummy = new Fixation(this.gameObject, null, new Vector2(5, 5));
        this.gameObject.AddComponent<DummyTechnique>();
        this.gameObject.GetComponent<DummyTechnique>().Start();

        this.SimulateFixation(fixDummy);
        var settingsActual = ServiceProvider.Get<GestureDetectionService>().GetSubscribedTechniques()
            .First(g => g.InteractionTechnique.Equals(this.gameObject.GetComponent<DummyTechnique>()));

        // check if fixation is added to the observed fixation by comparing the values
        Assert.IsTrue(
            settingsActual.ObservedFixations[0].Equals(fixDummy.Position),
            "Assert Fail: ObservedFixation is not equal to the actual fixation. It's {0} instead {1}",
            settingsActual.ObservedFixations[0],
            fixDummy.Position);
        Assert.IsTrue(
            settingsActual.ReferenceDistance.Equals(1),
            "Assert Fail: ReferenceDistance is not correctly set. It's {0} instead 1",
            settingsActual.ReferenceDistance);
        Assert.IsTrue(
            ServiceProvider.Get<GestureDetectionService>().GetLastFixation().Equals(new Vector2(5, 5)),
            "Asser Fail: LastFixation is not correctly set though there is a subscribed technique.");
    }

    /// <summary>
    /// Unit Test Case for OnFixStarted-method. 
    /// Condition: GestureDetectionService has a subscribed technique and two fixation occurs.
    /// Result: Both fixation are recognized as valid points by the technique.
    /// </summary>
    [Test]
    public void OnFixStartedWithSubscribedTechniqueAndTwoFixations()
    {
        // create two dummy fixation for testing
        var fixDummy1 = new Fixation(this.gameObject, null, new Vector2(0, 0));
        var fixDummy2 = new Fixation(this.gameObject, null, new Vector2(0, 1.1f));
        this.gameObject.AddComponent<DummyTechnique>();
        this.gameObject.GetComponent<DummyTechnique>().Start();

        this.SimulateFixation(fixDummy1);
        this.SimulateFixation(fixDummy2);
        var settingsActual = ServiceProvider.Get<GestureDetectionService>().GetSubscribedTechniques()
            .First(g => g.InteractionTechnique.Equals(this.gameObject.GetComponent<DummyTechnique>()));

        var distExpect = Vector2.Distance(fixDummy1.Position, fixDummy2.Position);

        // check if fixations are recognized and the values are set correctly
        Assert.IsTrue(
            settingsActual.ObservedFixations.Count.Equals(2),
            "Asser Fail: The amount of Fixation in ObservedFixation is not correct. It's {0} instead 2.",
            settingsActual.ObservedFixations.Count);
        Assert.IsTrue(
            settingsActual.ObservedFixations[1].Equals(fixDummy2.Position),
            "Assert Fail: ObservedFixation's last Fixation is not equal to the actual fixation. It's {0} instead {1}.",
            settingsActual.ObservedFixations[1],
            fixDummy2.Position);
        Assert.IsTrue(
            settingsActual.ReferenceDistance.Equals(distExpect),
            "Assert Fail: ReferenceDistance is not correctly set. It's {0} instead {1}.",
            settingsActual.ReferenceDistance,
            distExpect);
        Assert.IsTrue(
            ServiceProvider.Get<GestureDetectionService>().GetLastFixation().Equals(fixDummy2.Position),
            "Asser Fail: LastFixation is not correctly set though there is a subscribed technique.");
        Assert.IsFalse(
            this.gameObject.GetComponent<DummyTechnique>().IsDone(),
            "Assert Fail: Gesture is completed though it should not be");
    }

    /// <summary>
    /// Unit Test Case for OnFixStarted-method. 
    /// Condition: GestureDetectionService has a subscribed technique and a complete gesture occurs.
    /// Result: All fixations are recognized as valid points and the gesture of the technique is marked as completed.
    /// </summary>
    [Test]
    public void OnFixStartedWithSubscribedTechniqueAndCompletedGesture()
    {
        // create the occuring gesture represented by three fixations
        var fixDummy1 = new Fixation(this.gameObject, null, new Vector2(0, 0));
        var fixDummy2 = new Fixation(this.gameObject, null, new Vector2(0, 2));
        var fixDummy3 = new Fixation(this.gameObject, null, new Vector2(3.8f, 2));
        this.gameObject.AddComponent<DummyTechnique>();
        this.gameObject.GetComponent<DummyTechnique>().Start();

        this.SimulateFixation(fixDummy1);
        this.SimulateFixation(fixDummy2);
        this.SimulateFixation(fixDummy3);

        // check if gesture is completed
        Assert.IsTrue(
            this.gameObject.GetComponent<DummyTechnique>().IsDone(),
            "Assert Fail: Gesture is not completed though it should be");
    }

    /// <summary>
    /// Unit Test Case for OnFixStarted-method. 
    /// Condition: GestureDetectionService has two subscribed techniques and a complete gesture occurs.
    /// Result: All fixations are recognized as valid points and the gesture of the techniques are marked as completed.
    /// </summary>
    [Test]
    public void OnFixStartedWithTwoTechniqueAndCompletedGestures()
    {
        // create the occuring gesture represented by three fixations
        var fixDummy1 = new Fixation(this.gameObject, null, new Vector2(0, 0));
        var fixDummy2 = new Fixation(this.gameObject, null, new Vector2(0, 2));
        var fixDummy3 = new Fixation(this.gameObject, null, new Vector2(4, 2));

        this.gameObject.AddComponent<DummyTechnique>();
        this.gameObject.GetComponent<DummyTechnique>().Start();
        GameObject gameObject2 = new GameObject();
        gameObject2.AddComponent<DummyTechnique>();
        gameObject2.GetComponent<DummyTechnique>().Start();

        this.SimulateFixation(fixDummy1);
        this.SimulateFixation(fixDummy2);
        this.SimulateFixation(fixDummy3);

        // check if gesture is completed for both techniques
        Assert.IsTrue(
            this.gameObject.GetComponent<DummyTechnique>().IsDone(),
            "Assert Fail: First gesture is not completed though it should be");
        Assert.IsTrue(
            gameObject2.GetComponent<DummyTechnique>().IsDone(),
            "Assert Fail: Second gesture is not completed though it should be");
    }

    /// <summary>
    /// Unit Test Case for OnFixStarted-method. 
    /// Condition: GestureDetectionService has two subscribed techniques and a complete gesture occurs.
    /// Result: All fixations are recognized as valid points and the gesture of the first technique is marked as 
    /// completed, while the second gesture is aborted.
    /// </summary>
    [Test]
    public void OnFixStartedWithTwoTechniqueAndOneCompletedOneAbortedGestures()
    {
        // create the occuring gesture represented by three fixations
        var fixDummy1 = new Fixation(this.gameObject, null, new Vector2(0, 0));
        var fixDummy2 = new Fixation(this.gameObject, null, new Vector2(0, 2));
        var fixDummy3 = new Fixation(this.gameObject, null, new Vector2(4, 2));

        this.gameObject.AddComponent<DummyTechnique>();
        this.gameObject.GetComponent<DummyTechnique>().Start();
        this.gameObject.AddComponent<DummyTechnique2>();
        this.gameObject.GetComponent<DummyTechnique2>().Start();
        
        this.SimulateFixation(fixDummy1);
        this.SimulateFixation(fixDummy2);
        this.SimulateFixation(fixDummy3);

        // check if gesture is completed for both techniques
        Assert.IsTrue(
            this.gameObject.GetComponent<DummyTechnique>().IsDone(),
            "Assert Fail: First gesture is not completed though it should be");
        Assert.IsFalse(
            this.gameObject.GetComponent<DummyTechnique2>().IsDone(),
            "Assert Fail: Second gesture is completed though it should not be");
    }

    /// <summary>
    /// Unit Test Case for OnFixUpdate-method. It's a unit test to test explicitly the OnFixUpdate method. 
    /// Condition: GestureDetectionService has a subscribed techniques and a fixation is active.
    /// Result: Nothing should happen during the fixation.
    /// </summary>
    [Test]
    public void OnFixUpdatedShouldDoNothing()
    {
        // create a fixation which occours
        var fixDummy = new Fixation(this.gameObject, null, new Vector2(0, 0));

        this.gameObject.AddComponent<DummyTechnique>();
        this.gameObject.GetComponent<DummyTechnique>().Start();

        ServiceProvider.Get<GestureDetectionService>().OnFixStarted(fixDummy);
        var settingsAfterStart = ServiceProvider.Get<GestureDetectionService>().GetSubscribedTechniques()
            .First(g => g.InteractionTechnique.Equals(this.gameObject.GetComponent<DummyTechnique>()));
        var lastFixation = ServiceProvider.Get<GestureDetectionService>().GetLastFixation();

        ServiceProvider.Get<GestureDetectionService>().OnFixUpdate();
        var settingsActual = ServiceProvider.Get<GestureDetectionService>().GetSubscribedTechniques()
            .First(g => g.InteractionTechnique.Equals(this.gameObject.GetComponent<DummyTechnique>()));

        // check if fixation is added to the observed fixation by comparing the values
        Assert.IsTrue(
            settingsAfterStart.ObservedFixations[0].Equals(settingsActual.ObservedFixations[0]),
            "Assert Fail: ObservedFixation changed during OnFixUpdate, which should not happen. It's {0} instead {1}",
            settingsAfterStart.ObservedFixations[0],
            settingsActual.ObservedFixations[0]);
        Assert.IsTrue(
            settingsAfterStart.ReferenceDistance.Equals(settingsActual.ReferenceDistance),
            "Assert Fail: ReferenceDistance changed during OnFixUpdate, which should not happen. It's {0} instead {1}",
            settingsAfterStart.ReferenceDistance,
            settingsActual.ReferenceDistance);
        Assert.IsTrue(
            ServiceProvider.Get<GestureDetectionService>().GetLastFixation().Equals(lastFixation),
            "Asser Fail: LastFixation changed during OnFixUpdate, which should not happen.");
    }

    /// <summary>
    /// Unit Test Case for OnFixEnded-method. It's a unit test to test explicitly the OnFixEnded method. 
    /// Condition: GestureDetectionService has a subscribed techniques and a fixation ends.
    /// Result: Nothing should happen when the fixation ends.
    /// </summary>
    [Test]
    public void OnFixEndedShouldDoNothing()
    {
        // create a fixation which occours
        var fixDummy = new Fixation(this.gameObject, null, new Vector2(0, 0));

        this.gameObject.AddComponent<DummyTechnique>();
        this.gameObject.GetComponent<DummyTechnique>().Start();

        ServiceProvider.Get<GestureDetectionService>().OnFixStarted(fixDummy);
        var settingsAfterStart = ServiceProvider.Get<GestureDetectionService>().GetSubscribedTechniques()
            .First(g => g.InteractionTechnique.Equals(this.gameObject.GetComponent<DummyTechnique>()));
        var lastFixation = ServiceProvider.Get<GestureDetectionService>().GetLastFixation();

        ServiceProvider.Get<GestureDetectionService>().OnFixEnded();
        var settingsActual = ServiceProvider.Get<GestureDetectionService>().GetSubscribedTechniques()
            .First(g => g.InteractionTechnique.Equals(this.gameObject.GetComponent<DummyTechnique>()));

        // check if fixation is added to the observed fixation by comparing the values
        Assert.IsTrue(
            settingsAfterStart.ObservedFixations[0].Equals(settingsActual.ObservedFixations[0]),
            "Assert Fail: ObservedFixation changed during OnFixEnded, which should not happen. It's {0} instead {1}",
            settingsAfterStart.ObservedFixations[0],
            settingsActual.ObservedFixations[0]);
        Assert.IsTrue(
            settingsAfterStart.ReferenceDistance.Equals(settingsActual.ReferenceDistance),
            "Assert Fail: ReferenceDistance changed during OnFixEnded, which should not happen. It's {0} instead {1}",
            settingsAfterStart.ReferenceDistance,
            settingsActual.ReferenceDistance);
        Assert.IsTrue(
            ServiceProvider.Get<GestureDetectionService>().GetLastFixation().Equals(lastFixation),
            "Asser Fail: LastFixation changed during OnFixEnded, which should not happen.");
    }

    /// <summary>
    /// This helper-method is used to simulate the fixation calls of the FixationDetectionService.
    /// </summary>
    /// <param name="fixation">
    /// The fixation which should simulated.
    /// </param>
    private void SimulateFixation(Fixation fixation)
    {
        ServiceProvider.Get<GestureDetectionService>().OnFixStarted(fixation);
        ServiceProvider.Get<GestureDetectionService>().OnFixUpdate();
        ServiceProvider.Get<GestureDetectionService>().OnFixEnded();
    }

    // ----------------------- Dummy Classes --------------------------------

    /// <summary>
    /// A dummy technique with a dummy gesture.
    /// It's used to test some methods of GestureDetectionService.
    /// </summary>
    public class DummyTechnique : MonoBehaviour, IGestureInteractionTechnique
    {
        // variables are public so they can be easily compared
        private GestureDetectionService.Gesture gestureDummy;
        private bool done;

        /// <summary>
        /// The start.
        /// </summary>
        public void Start()
        {
            // create dummy gesture for testing
            this.gestureDummy = new GestureDetectionService.Gesture(
                new[] { 1.0f, 2.0f },
                new[] { 0f, 270f });
            this.done = false;

            ServiceProvider.Get<GestureDetectionService>().Subscribe(this, this.gestureDummy, 30, 0.5);
        }

        /// <summary>
        /// Subscribe to the GestureDetectionService.
        /// </summary>
        public void Subscribe()
        {
            ServiceProvider.Get<GestureDetectionService>().Subscribe(this, this.gestureDummy, 30, 0.5);
        }

        /// <summary>
        /// Unsubscribe from the GestureDetectionService.
        /// </summary>
        public void Unsubscribe()
        {
            ServiceProvider.Get<GestureDetectionService>().Unsubscribe(this);
        }

        /// <summary>
        /// The on gesture.
        /// </summary>
        public void OnGesture()
        {
            // gesture is completed successful
            this.done = true;
        }

        /// <summary>
        /// Returns the dummy-gesture of this technique.
        /// </summary>
        /// <returns>
        /// The <see cref="GestureDetectionService.Gesture"/>.
        /// </returns>
        public GestureDetectionService.Gesture GetGesture()
        {
            return this.gestureDummy;
        }

        /// <summary>
        /// Returns if the gesture of this technique is completed.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsDone()
        {
            return this.done;
        }
    }

    /// <summary>
    /// A second dummy technique with a dummy gesture.
    /// It's used to test some methods of GestureDetectionService.
    /// </summary>
    public class DummyTechnique2 : MonoBehaviour, IGestureInteractionTechnique
    {
        // variables are public so they can be easily compared
        private GestureDetectionService.Gesture gestureDummy;
        private bool done;

        /// <summary>
        /// The start.
        /// </summary>
        public void Start()
        {
            // create dummy gesture for testing
            this.gestureDummy = new GestureDetectionService.Gesture(
                new[] { 1.0f, 2.0f },
                new[] { 0f, 90f });
            this.done = false;

            ServiceProvider.Get<GestureDetectionService>().Subscribe(this, this.gestureDummy, 30, 0.5);
        }

        /// <summary>
        /// Subscribe to the GestureDetectionService.
        /// </summary>
        public void Subscribe()
        {
            ServiceProvider.Get<GestureDetectionService>().Subscribe(this, this.gestureDummy, 30, 0.5);
        }

        /// <summary>
        /// Unsubscribe from the GestureDetectionService.
        /// </summary>
        public void Unsubscribe()
        {
            ServiceProvider.Get<GestureDetectionService>().Unsubscribe(this);
        }

        /// <summary>
        /// The on gesture.
        /// </summary>
        public void OnGesture()
        {
            // gesture is completed successful
            this.done = true;
        }

        /// <summary>
        /// Returns the dummy-gesture of this technique.
        /// </summary>
        /// <returns>
        /// The <see cref="GestureDetectionService.Gesture"/>.
        /// </returns>
        public GestureDetectionService.Gesture GetGesture()
        {
            return this.gestureDummy;
        }

        /// <summary>
        /// Returns if the gesture of this technique is completed.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsDone()
        {
            return this.done;
        }
    }
}
