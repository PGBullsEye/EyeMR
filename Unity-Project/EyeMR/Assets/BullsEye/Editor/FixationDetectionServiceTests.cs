// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FixationDetectionServiceTests.cs" company="PGBullsEye">
//   Author: Daniela Betzl, Henrik Reichmann
// </copyright>
// <summary>
//   Defines the FixationDetectionServiceTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;

using BullsEye.Scripts.Interfaces;
using BullsEye.Scripts.Model.Fixation;
using BullsEye.Scripts.Receiving;
using BullsEye.Scripts.Services;
using BullsEye.Scripts.Streaming;

using NUnit.Framework;

using UnityEngine;

/// <summary>
/// This class contains unit tests for the FixationDetectionService
/// </summary>
[TestFixture]
public class FixationDetectionServiceTests
{
    private GameObject gameObject;

    /// <summary>
    /// Setting up the objects for the tests and instantiate the FixationDetectionService.
    /// </summary>
    [TestFixtureSetUp]
    public void Init()
    {
        this.gameObject = new GameObject("ToolboxTests");
        this.gameObject.AddComponent<ServiceProvider>().Start();
        this.gameObject.AddComponent<SetupStreamingCamera>().Update();
        ServiceProvider.Get<FixationDetectionService>().Start();
    }

    /// <summary>
    /// Cleaning up the objects after the tests.
    /// </summary>
    [TearDown]
    public void Dispose()
    {
        ServiceProvider.Get<FixationDetectionService>().Reset();
        foreach (PupilListener p in this.gameObject.GetComponents<PupilListener>())
        {
            GameObject.DestroyImmediate(p);
        }
    }

    /// <summary>
    /// The cleanup.
    /// </summary>
    [TestFixtureTearDown]
    public void Cleanup()
    {
        ServiceProvider.Destroy<FixationDetectionService>();
        GameObject.DestroyImmediate(ServiceProvider.Instance);
        GameObject.DestroyImmediate(this.gameObject);
        Application.Quit();
    }

    /// <summary>
    /// Unit Test Case for Start method. 
    /// Condition: FixationDetectionService is instantiated.
    /// Result: FixationDetectionService is subscribed correctly.
    /// </summary>
    [Test]
    public void StartWasExecuted()
    {
        var listActual = ServiceProvider.Get<FixationDetectionService>().GetSubscribedTechniques();

        if (listActual == null)
        {
            Assert.Fail("Assert Fail: FixationDetectionService got none subscribed scripts.");
        }

        if (listActual.Any())
        {
            Assert.Fail(
                "Assert Fail: FixationDetectionService got wrong amount of subscribed scripts. There are "
                + listActual.Count + "subscribers.");
        }
    }

    /// <summary>
    /// Unit Test Case for Subscribe method. 
    /// Condition: No script is subscribed to FixationDetectionService. Service is started.
    /// Result: One dummy technique subscribes successfully to FixationDetectionService.
    /// </summary>
    [Test]
    public void SubscribeSuccessfullAndServiceIsStarted()
    {
        // create dummy technique and subscribe to FixationDetectionService
        this.gameObject.AddComponent<DummyFixation>();
        var fixation = this.gameObject.GetComponent<DummyFixation>();
        fixation.SubscribeObjectBased();

        var listActual = ServiceProvider.Get<FixationDetectionService>().GetSubscribedTechniques();

        if (listActual == null || listActual.Count() != 1)
        {
            Assert.Fail(
                "Assert Fail: FixationDetectionService got none or more than 1 subscribed technique, which should not happen.");
        }

        // check if DummyFixation subscribed correctly to the service and the list entry contains the correct values
        Assert.IsTrue(
            listActual.Any(f => f.InteractionTechnique.Equals(this.gameObject.GetComponent<DummyFixation>())),
            "Assert Fail: FixationTechnique did not subscribe to FixationDetectionService correctly.");
    }

    /// <summary>
    /// Unit Test Case for Subscribe-method. 
    /// Condition: No techniques is subscribed to PursuitDetectionService.
    /// Result: Two dummy techniques subscribe successfully to PursuitDetectionService.
    /// </summary>
    [Test]
    public void SubscribeTwoSuccessful()
    {
        DummyFixation dummy = this.gameObject.AddComponent<DummyFixation>();
        DummyFixation dummy2 = this.gameObject.AddComponent<DummyFixation>();
        dummy.SubscribeObjectBased();
        dummy2.SubscribeObjectBased();

        var fixations = ServiceProvider.Get<FixationDetectionService>().GetSubscribedTechniques();

        if (fixations == null)
        {
            Assert.Fail("Assert Fail: FixationDetectionService got none subscribed fixations, which should not happen.");
        }

        if (fixations.Count != 2)
        {
            Assert.Fail("Assert Fail: FixationDetectionService got {0} instead of 2 subscribed fixations, which should not happen.", fixations.Count);
        }
    }

    /// <summary>
    /// Unit Test Case for Subscribe-method. 
    /// Condition: A technique is subscribed to PursuitDetectionService and it tries to subscribe a second time.
    /// Result: There still exist just one pursuit of this technique at the PursuitDetectionService.
    /// </summary>
    [Test]
    public void SubscribeNotSecondTime()
    {
        DummyFixation dummy = this.gameObject.AddComponent<DummyFixation>();
        dummy.SubscribeObjectBased();

        // try to subscribe a second time
        dummy.SubscribeObjectBased();

        var fixations = ServiceProvider.Get<FixationDetectionService>().GetSubscribedTechniques();

        if (fixations == null)
        {
            Assert.Fail("Assert Fail: FixationDetectionService got none subscribed fixations, which should not happen.");
        }

        if (fixations.Count != 1)
        {
            Assert.Fail("Assert Fail: FixationDetectionService got {0} instead of 1 subscribed fixations. The fixation technique could subscribe twice, which should not happen.", fixations.Count);
        }
    }

    /// <summary>
    /// Unit Test Case for Subscribe method. 
    /// Condition: No script is subscribed to FixationDetectionService. Service is started.
    /// Result: One dummy technique subscribes and unsubscribes successfully to FixationDetectionService.
    /// </summary>
    [Test]
    public void UnsubscribeSuccessfullAndServiceIsStarted()
    {
        // create dummy technique and subscribe to FixationDetectionService
        this.gameObject.AddComponent<DummyFixation>();
        var fixation = this.gameObject.GetComponent<DummyFixation>();
        fixation.SubscribeObjectBased();

        var listActual = ServiceProvider.Get<FixationDetectionService>().GetSubscribedTechniques();

        if (listActual == null || listActual.Count() != 1)
        {
            Assert.Fail(
                "Assert Fail: FixationDetectionService got none or more than 1 subscribed technique, which should not happen.");
        }

        // check if DummyFixation subscribed correctly to the service and the list entry contains the correct values
        Assert.IsTrue(
            listActual.Any(f => f.InteractionTechnique.Equals(this.gameObject.GetComponent<DummyFixation>())),
            "Assert Fail: FixationTechnique did not subscribe to FixationDetectionService correctly.");

        ServiceProvider.Get<FixationDetectionService>().Unsubscribe(fixation);

        // check if DummyFixation subscribed correctly to the service and the list entry contains the correct values
        Assert.IsFalse(
            listActual.Any(f => f.InteractionTechnique.Equals(this.gameObject.GetComponent<DummyFixation>())),
            "Assert Fail: FixationTechnique did not subscribe to FixationDetectionService correctly.");
    }

    /// <summary>
    /// Unit Test Case for Update method. 
    /// Condition: DummyFixation is started. 
    /// Result: Start-method of DummyFixation wasn't called so there are no subscriptions.
    /// </summary>
    [Test]
    public void CheckSubscribtionWithoutStartFixation()
    {
        // create dummy technique and subscribe to FixationDetectionService
        this.gameObject.AddComponent<DummyFixation>();
        var listActual = ServiceProvider.Get<FixationDetectionService>().GetSubscribedTechniques();

        if (listActual != null && listActual.Count().Equals(0))
        {
            Assert.True(listActual.Count().Equals(0), "Assert Fail: One Fixation is subscribed. ");
        }
    }

    /// <summary>
    /// Unit Test Case for Update method. 
    /// Condition: No script is subscribed to FixationDetectionService. Service is started.
    /// Result: One dummy fixation subscribes successfully to FixationDetectionService, but none of the methods is called.
    /// </summary>
    [Test]
    public void UpdateWithSubscribedScriptWithoutFixation()
    {
        Assert.IsFalse(
            ServiceProvider.Get<FixationDetectionService>().GetSubscribedTechniques().Any(),
            "Assert Fail: A Script is subscribed to FixationDetectionService which should not be");

        this.gameObject.AddComponent<SetupStreamingCamera>().Update();

        // create dummy technique and subscribe to FixationDetectionService
        var fixation = this.gameObject.AddComponent<DummyFixation>();
        fixation.SubscribeNonObjectBased();
        var pupil = ServiceProvider.Get<DummyPupilListener>();
        pupil.CreateFixation = false;

        for (int i = 0; i < 100; i++)
        {
            ServiceProvider.Get<FixationDetectionService>().FixedUpdate();
        }

        Assert.False(fixation.CalledFixStart, "Assert Fail: Method OnFixStart was called though it should not be");
        Assert.False(fixation.CalledFixUpdate, "Assert Fail: Method OnFixUpdate was called though it should not be");
        Assert.False(fixation.CalledFixEnded, "Assert Fail: Method OnFixEnded was called though it should not be");
    }

    /// <summary>
    /// Unit Test Case for the Update Method
    /// Condition: No script is subscribed to FixationDetectionService. Service is started.
    /// Result: One dummy fixation subscribes successfully to FixationDetectionService, and all <see cref="IFixationInteractionTechnique"/> methods are called.
    /// </summary>
    [Test]
    public void UpdateWithTechniqueNonObjectBased()
    {
        Assert.IsFalse(
            ServiceProvider.Get<FixationDetectionService>().GetSubscribedTechniques().Any(),
            "Assert Fail: A Script is subscribed to FixationDetectionService which should not be");

        // create dummy technique and subscribe to FixationDetectionService
        var fixation = this.gameObject.AddComponent<DummyFixation>();
        fixation.SubscribeNonObjectBased();
        var pupil = ServiceProvider.Get<DummyPupilListener>();
        pupil.CreateFixation = true;

        for (int i = 0; i < 50; i++)
        {
            ServiceProvider.Get<FixationDetectionService>().FixedUpdate();
        }

        Assert.True(fixation.CalledFixStart, "Assert Fail: Method OnFixStart was not called though it should be");

        ServiceProvider.Get<FixationDetectionService>().FixedUpdate();

        Assert.True(fixation.CalledFixUpdate, "Assert Fail: Method OnFixUpdate was not called though it should be");

        pupil.CreateFixation = false;
        for (int i = 0; i < 50; i++)
        {
            ServiceProvider.Get<FixationDetectionService>().FixedUpdate();
        }

        Assert.True(fixation.CalledFixEnded, "Assert Fail: Method OnFixEnded was not called though it should be");
    }

    /// <summary>
    /// Unit Test Case for the Update Method
    /// Condition: No script is subscribed to FixationDetectionService. Service is started.
    /// Result: One object based dummy fixation subscribes successfully to FixationDetectionService, and all <see cref="IFixationInteractionTechnique"/> methods are called.
    /// </summary>
    [Test]
    public void UpdateWithTechniqueObjectBased()
    {
        Assert.IsFalse(
            ServiceProvider.Get<FixationDetectionService>().GetSubscribedTechniques().Any(),
            "Assert Fail: A Script is subscribed to FixationDetectionService which should not be");

        this.gameObject.transform.position = new Vector3(0, 0, 0);

        // create dummy technique and subscribe to FixationDetectionService
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = new Vector3(0, 0, 5);

        var fixation = go.AddComponent<DummyFixation>();
        fixation.SubscribeObjectBased();
        var pupil = ServiceProvider.Get<DummyPupilListener>();
        pupil.CreateFixation = true;

        for (int i = 0; i < 50; i++)
        {
            ServiceProvider.Get<FixationDetectionService>().FixedUpdate();
        }

        Assert.True(fixation.CalledFixStart, "Assert Fail: Method OnFixStart was not called though it should be");

        ServiceProvider.Get<FixationDetectionService>().FixedUpdate();

        Assert.True(fixation.CalledFixUpdate, "Assert Fail: Method OnFixUpdate was not called though it should be");

        pupil.CreateFixation = false;
        for (int i = 0; i < 50; i++)
        {
            ServiceProvider.Get<FixationDetectionService>().FixedUpdate();
        }

        Assert.True(fixation.CalledFixEnded, "Assert Fail: Method OnFixEnded was not called though it should be");
    }

    /// <summary>
    /// Unit Test Case for Update method. 
    /// Condition: No script is subscribed to FixationDetectionService. Service is started.
    /// Result: One object based dummy fixation subscribes successfully to FixationDetectionService, but none of the methods is called.
    /// </summary>
    [Test]
    public void UpdateWithTechniqueObjectBasedFixationOffObject()
    {
        Assert.IsFalse(
            ServiceProvider.Get<FixationDetectionService>().GetSubscribedTechniques().Any(),
            "Assert Fail: A Script is subscribed to FixationDetectionService which should not be");

        this.gameObject.transform.position = new Vector3(0, 0, 0);

        // create dummy technique and subscribe to FixationDetectionService
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = new Vector3(0, 0, 5);

        var fixation = go.AddComponent<DummyFixation>();
        fixation.SubscribeObjectBased();
        var pupil = ServiceProvider.Get<DummyPupilListener>();
        pupil.CreateFixation = true;
        pupil.OffCenterFixation = true;

        for (int i = 0; i < 100; i++)
        {
            ServiceProvider.Get<FixationDetectionService>().FixedUpdate();
        }

        Assert.False(fixation.CalledFixStart, "Assert Fail: Method OnFixStart was called though it should not be");
        Assert.False(fixation.CalledFixUpdate, "Assert Fail: Method OnFixUpdate was called though it should not be");
        Assert.False(fixation.CalledFixEnded, "Assert Fail: Method OnFixEnded was called though it should not be");
    }

    /// <summary>
    /// Unit Test Case for the Update Method
    /// Condition: No script is subscribed to FixationDetectionService. Service is started.
    /// Result: One area based dummy fixation subscribes successfully to FixationDetectionService, and all <see cref="IFixationInteractionTechnique"/> methods are called.
    /// </summary>
    [Test]
    public void UpdateWithTechniqueAreaBased()
    {
        Assert.IsFalse(
            ServiceProvider.Get<FixationDetectionService>().GetSubscribedTechniques().Any(),
            "Assert Fail: A Script is subscribed to FixationDetectionService which should not be");

        // create dummy technique and subscribe to FixationDetectionService
        var fixation = this.gameObject.AddComponent<DummyFixation>();
        fixation.SubscribeAreaBased();
        var pupil = ServiceProvider.Get<DummyPupilListener>();
        pupil.CreateFixation = true;

        for (int i = 0; i < 50; i++)
        {
            ServiceProvider.Get<FixationDetectionService>().FixedUpdate();
        }

        Assert.True(fixation.CalledFixStart, "Assert Fail: Method OnFixStart was not called though it should be");

        ServiceProvider.Get<FixationDetectionService>().FixedUpdate();

        Assert.True(fixation.CalledFixUpdate, "Assert Fail: Method OnFixUpdate was not called though it should be");

        pupil.CreateFixation = false;
        for (int i = 0; i < 50; i++)
        {
            ServiceProvider.Get<FixationDetectionService>().FixedUpdate();
        }

        Assert.True(fixation.CalledFixEnded, "Assert Fail: Method OnFixEnded was not called though it should be");
    }

    /// <summary>
    /// Unit Test Case for Update method. 
    /// Condition: No script is subscribed to FixationDetectionService. Service is started.
    /// Result: One object based dummy fixation subscribes successfully to FixationDetectionService, but none of the methods is called.
    /// </summary>
    [Test]
    public void UpdateWithTechniqueAreaBasedFixationOutOfArea()
    {
        Assert.IsFalse(
            ServiceProvider.Get<FixationDetectionService>().GetSubscribedTechniques().Any(),
            "Assert Fail: A Script is subscribed to FixationDetectionService which should not be");

        // create dummy technique and subscribe to FixationDetectionService
        var fixation = this.gameObject.AddComponent<DummyFixation>();
        fixation.SubscribeAreaBased();
        var pupil = ServiceProvider.Get<DummyPupilListener>();
        pupil.CreateFixation = true;
        pupil.OffCenterFixation = true;

        for (int i = 0; i < 100; i++)
        {
            ServiceProvider.Get<FixationDetectionService>().FixedUpdate();
        }

        Assert.False(fixation.CalledFixStart, "Assert Fail: Method OnFixStart was called though it should not be");
        Assert.False(fixation.CalledFixUpdate, "Assert Fail: Method OnFixUpdate was called though it should not be");
        Assert.False(fixation.CalledFixEnded, "Assert Fail: Method OnFixEnded was called though it should not be");
    }

    /// <summary>
    /// Unit Test Case for the Update Method
    /// Condition: No script is subscribed to FixationDetectionService. Service is started.
    /// Result: One area and object based dummy fixation subscribes successfully to FixationDetectionService, and all <see cref="IFixationInteractionTechnique"/> methods are called.
    /// </summary>
    [Test]
    public void UpdateWithTechniqueAreaAndObjectBased()
    {
        Assert.IsFalse(
            ServiceProvider.Get<FixationDetectionService>().GetSubscribedTechniques().Any(),
            "Assert Fail: A Script is subscribed to FixationDetectionService which should not be");

        this.gameObject.transform.position = new Vector3(0, 0, 0);

        // create dummy technique and subscribe to FixationDetectionService
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = new Vector3(0, 0, 5);

        var fixation = go.AddComponent<DummyFixation>();
        fixation.SubscribeAreaObjectBased();
        var pupil = ServiceProvider.Get<DummyPupilListener>();
        pupil.CreateFixation = true;

        for (int i = 0; i < 50; i++)
        {
            ServiceProvider.Get<FixationDetectionService>().FixedUpdate();
        }

        Assert.True(fixation.CalledFixStart, "Assert Fail: Method OnFixStart was not called though it should be");

        ServiceProvider.Get<FixationDetectionService>().FixedUpdate();

        Assert.True(fixation.CalledFixUpdate, "Assert Fail: Method OnFixUpdate was not called though it should be");

        pupil.CreateFixation = false;
        for (int i = 0; i < 50; i++)
        {
            ServiceProvider.Get<FixationDetectionService>().FixedUpdate();
        }

        Assert.True(fixation.CalledFixEnded, "Assert Fail: Method OnFixEnded was not called though it should be");
    }

    /// <summary>
    /// Unit Test Case for Update method. 
    /// Condition: No script is subscribed to FixationDetectionService. Service is started.
    /// Result: One area and object based dummy fixation subscribes successfully to FixationDetectionService, but none of the methods is called.
    /// </summary>
    [Test]
    public void UpdateWithTechniqueAreaAndObjectBasedFixationOutOfArea()
    {
        Assert.IsFalse(
            ServiceProvider.Get<FixationDetectionService>().GetSubscribedTechniques().Any(),
            "Assert Fail: A Script is subscribed to FixationDetectionService which should not be");

        this.gameObject.transform.position = new Vector3(0, 0, 0);

        // create dummy technique and subscribe to FixationDetectionService
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = new Vector3(0, 0, 5);

        var fixation = go.AddComponent<DummyFixation>();
        fixation.SubscribeAreaBased();
        var pupil = ServiceProvider.Get<DummyPupilListener>();
        pupil.CreateFixation = true;
        pupil.OffCenterFixation = true;

        for (int i = 0; i < 100; i++)
        {
            ServiceProvider.Get<FixationDetectionService>().FixedUpdate();
        }

        Assert.False(fixation.CalledFixStart, "Assert Fail: Method OnFixStart was called though it should not be");
        Assert.False(fixation.CalledFixUpdate, "Assert Fail: Method OnFixUpdate was called though it should not be");
        Assert.False(fixation.CalledFixEnded, "Assert Fail: Method OnFixEnded was called though it should not be");
    }

    // -------------------------------- Dummy Classes -------------------------------

    /// <summary>
    /// The dummy pupil listener.
    /// </summary>
    public class DummyPupilListener : PupilListener
    {
        public bool CreateFixation;

        public bool OffCenterFixation;

        private int counter;

        /// <summary>
        /// Overrides the method from <see cref="PupilListener"/> to provide data usable in Unit Tests.
        /// </summary>
        /// <param name="filtered">
        /// Whether or not the data should be returned.
        /// </param>
        /// <returns>
        /// The <see cref="Vector2"/>.
        /// </returns>
        public override Vector2 GetCoordinates(bool filtered = true)
        {
            var cam = ServiceProvider.Instance.GetComponent<Camera>();
            var width = cam.pixelWidth;
            var height = cam.pixelHeight;

            if (!this.CreateFixation)
            {
                switch (this.counter)
                {
                    case 0:
                        this.counter++;
                        return new Vector2(0, 0);
                    case 1:
                        this.counter++;
                        return new Vector2(width, 0);
                    case 2:
                        this.counter++;
                        return new Vector2(0, height);
                    default:
                        this.counter = 0;
                        return new Vector2(width, height);
                }
            }

            return this.OffCenterFixation ? new Vector2(0, 0) : new Vector2(width / 2.0f, height / 2.0f);
        }
    }
        
    /// <summary>
    ///   Simple dummy of an interaction technique using fixations. Moves the GameObject it is attached to to the left as long as it is fixated.
    /// </summary>
    public class DummyFixation : MonoBehaviour, IFixationInteractionTechnique
    {
        // booleans to check if the specific methods were called
        public bool CalledFixEnded;

        public bool CalledFixUpdate;

        public bool CalledFixStart;

        /// <summary>
        /// Gets the Fixation.
        /// </summary>
        public Fixation GetFixation { get; private set; }

        /// <summary>
        /// Implemented method of <see cref="IFixationInteractionTechnique"/>. Just logs the end of a Fixation.
        /// </summary>
        public void OnFixEnded()
        {
            Debug.Log("DummyFixation End ");
            this.GetFixation = null;
            this.CalledFixEnded = true;
        }

        /// <summary>
        /// Implemented method of <see cref="IFixationInteractionTechnique"/>. Just logs the start of a new Fixation.
        /// </summary>
        /// <param name="fix">
        /// The new fixation.
        /// </param>
        public void OnFixStarted(Fixation fix)
        {
            Debug.Log("DummyFixation Started");
            this.GetFixation = fix;
            this.CalledFixStart = true;
        }

        /// <summary>
        /// Implemented method of <see cref="IFixationInteractionTechnique"/>. Is called every frame a fixations continues.
        /// Moves the GameObject it is attached to to the left.
        /// </summary>
        public void OnFixUpdate()
        {
            this.CalledFixUpdate = true;
        }

        /// <summary>
        /// Subscribes the interaction technique to the <see cref="FixationDetectionService"/> as non-object-based technique.
        /// </summary>
        public void SubscribeNonObjectBased()
        {
            ServiceProvider.Get<FixationDetectionService>().Subscribe(this, false);
        }

        /// <summary>
        /// Subscribes the interaction technique to the <see cref="FixationDetectionService"/> as object-based technique.
        /// </summary>
        public void SubscribeObjectBased()
        {
            ServiceProvider.Get<FixationDetectionService>().Subscribe(this);
        }

        /// <summary>
        /// Subscribes the interaction technique to the <see cref="FixationDetectionService"/> as are-based technique.
        /// </summary>
        public void SubscribeAreaBased()
        {   
            ServiceProvider.Get<FixationDetectionService>().Subscribe(this, false, this.GetCenterArea());
        }

        /// <summary>
        /// Subscribes the interaction technique to the <see cref="FixationDetectionService"/> as area-based and object-based technique.
        /// </summary>
        public void SubscribeAreaObjectBased()
        {
            ServiceProvider.Get<FixationDetectionService>().Subscribe(this, true, this.GetCenterArea());
        }

        /// <summary>
        /// Returns an area in the middle of the attached camera.
        /// </summary>
        /// <returns>
        /// The <see cref="Vector2"/> array representing the area.
        /// </returns>
        private Vector2[] GetCenterArea()
        {
            var attachedCamera = ServiceProvider.Instance.GetComponent<Camera>();
            var width = attachedCamera.pixelWidth;
            var height = attachedCamera.pixelHeight;

            return new[]
                       {
                           new Vector2(width / 3.0f, height / 3.0f),
                           new Vector2(2 * width / 3.0f, 2 * height / 3.0f)
                       };
        }
    }
}