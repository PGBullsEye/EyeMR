// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PursuitDetectionServiceTests.cs"  company="PG BullsEye">
//   Author: Daniela Betzl
// </copyright>
// <summary>
//   This class contains unit tests for the PursuitDetectionService
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using BullsEye.Scripts.Interfaces;
using BullsEye.Scripts.Receiving;
using BullsEye.Scripts.Services;
using BullsEye.Scripts.Streaming;

using NUnit.Framework;

using UnityEngine;

/// <summary>
/// This class contains unit tests for the PursuitDetectionService
/// </summary>
[TestFixture]
public class PursuitDetectionServiceTests
{
    private GameObject serviceObject;
    private GameObject pursuitObject1;
    private GameObject pursuitObject2;
    private DummyPupilListener pupil;

    /// <summary>
    /// Setting up the objects for the tests and instantiate the GestureDetectionService.
    /// </summary>
    [TestFixtureSetUp]
    public void Init()
    {
        this.serviceObject = new GameObject("ServiceProviderHolder");
        this.serviceObject.AddComponent<ServiceProvider>().Start();
        this.serviceObject.AddComponent<SetupStreamingCamera>().Update();
        this.pupil = this.serviceObject.AddComponent<DummyPupilListener>();

        // Setup gameobjects for the dummy pursuit techniques
        this.pursuitObject1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        this.pursuitObject1.name = "Pursuit1";
        this.pursuitObject2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        this.pursuitObject2.name = "Pursuit2";
    }

    /// <summary>
    /// Cleaning up the objects after the tests.
    /// </summary>
    [TearDown]
    public void Dispose()
    {
        ServiceProvider.Get<PursuitDetectionService>().GetSubscribedTechniques().Clear();
        this.pupil.SetCounter();
    }

    /// <summary>
    /// The cleanup.
    /// </summary>
    [TestFixtureTearDown]
    public void Cleanup()
    {
        ServiceProvider.Destroy<PursuitDetectionService>();
        Object.DestroyImmediate(ServiceProvider.Instance);
        Object.DestroyImmediate(this.serviceObject);
        Object.DestroyImmediate(this.pursuitObject1);
        Object.DestroyImmediate(this.pursuitObject2);
    }

    /// <summary>
    /// Unit Test Case for Subscribe-method. 
    /// Condition: No techniques is subscribed to PursuitDetectionService.
    /// Result: One dummy technique subscribes successfully to PursuitDetectionService.
    /// </summary>
    [Test]
    public void SubscribeSuccessful()
    {
        DummyPursuit dummy = this.serviceObject.AddComponent<DummyPursuit>();
        dummy.Subscribe();

        var pursuits = ServiceProvider.Get<PursuitDetectionService>().GetSubscribedTechniques();

        if (pursuits == null)
        {
            Assert.Fail("Assert Fail: PursuitsDetectionService got none subscribed pursuits, which should not happen.");
        }

        if (pursuits.Count != 1)
        {
            Assert.Fail("Assert Fail: PursuitsDetectionService got {0} instead of 1 subscribed pursuit, which should not happen.", pursuits.Count);
        }
    }

    /// <summary>
    /// Unit Test Case for Subscribe-method. 
    /// Condition: No techniques is subscribed to PursuitDetectionService.
    /// Result: Two dummy techniques subscribe successfully to PursuitDetectionService.
    /// </summary>
    [Test]
    public void SubscribeTwoSuccessful()
    {
        DummyPursuit dummy = this.serviceObject.AddComponent<DummyPursuit>();
        DummyPursuit dummy2 = this.serviceObject.AddComponent<DummyPursuit>();
        dummy.Subscribe();
        dummy2.Subscribe();

        var pursuits = ServiceProvider.Get<PursuitDetectionService>().GetSubscribedTechniques();

        if (pursuits == null)
        {
            Assert.Fail("Assert Fail: PursuitsDetectionService got none subscribed pursuits, which should not happen.");
        }

        if (pursuits.Count != 2)
        {
            Assert.Fail("Assert Fail: PursuitsDetectionService got {0} instead of 2 subscribed pursuits, which should not happen.", pursuits.Count);
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
        DummyPursuit dummy = this.serviceObject.AddComponent<DummyPursuit>();
        dummy.Subscribe();

        // try to subscribe a second time
        dummy.Subscribe();

        var pursuits = ServiceProvider.Get<PursuitDetectionService>().GetSubscribedTechniques();

        if (pursuits == null)
        {
            Assert.Fail("Assert Fail: PursuitsDetectionService got none subscribed pursuits, which should not happen.");
        }

        if (pursuits.Count != 1)
        {
            Assert.Fail("Assert Fail: PursuitsDetectionService got {0} instead of 1 subscribed pursuit. The pursuit technique could subscribe twice, which should not happen.", pursuits.Count);
        }
    }

    /// <summary>
    /// Unit Test Case for Unsubscribe-method. 
    /// Condition: One technique is subscribed to the PursuitDetectionService.
    /// Result: The technique is successfully unsubscribed from the PursuitDetectionService.
    /// </summary>
    [Test]
    public void UnsubscribeOnlyTechniqueSuccessful()
    {
        DummyPursuit dummy = this.serviceObject.AddComponent<DummyPursuit>();
        dummy.Subscribe();

        // Check first if it's subscribed
        var pursuits = ServiceProvider.Get<PursuitDetectionService>().GetSubscribedTechniques();

        if (pursuits == null)
        {
            Assert.Fail("Assert Fail: PursuitsDetectionService got none subscribed pursuits, which should not happen.");
        }

        if (pursuits.Count != 1)
        {
            Assert.Fail("Assert Fail: PursuitsDetectionService got {0} instead of 1 subscribed pursuit, which should not happen.", pursuits.Count);
        }

        // Then unsubscribe
        dummy.Unsubscribe();
        pursuits = ServiceProvider.Get<PursuitDetectionService>().GetSubscribedTechniques();

        if (pursuits.Count != 0)
        {
            Assert.Fail("Assert Fail: The pursuit technique could not unsubscribe properly. Subscribed pursuits: {0}", pursuits.Count);
        }
    }

    /// <summary>
    /// Unit Test Case for Unsubscribe-method. 
    /// Condition: Two techniques are subscribed to the PursuitDetectionService.
    /// Result: The first technique is successfully unsubscribed from the PursuitDetectionService.
    /// </summary>
    [Test]
    public void UnsubscribeSuccessfulWithRemainingTechnique()
    {
        DummyPursuit dummy = this.serviceObject.AddComponent<DummyPursuit>();
        DummyPursuit dummy2 = this.serviceObject.AddComponent<DummyPursuit>();
        dummy.Subscribe();
        dummy2.Subscribe();

        var pursuits = ServiceProvider.Get<PursuitDetectionService>().GetSubscribedTechniques();

        // Check first if they're subscribed
        if (pursuits == null)
        {
            Assert.Fail("Assert Fail: PursuitsDetectionService got none subscribed pursuits, which should not happen.");
        }

        if (pursuits.Count != 2)
        {
            Assert.Fail("Assert Fail: PursuitsDetectionService got {0} instead of 2 subscribed pursuits, which should not happen.", pursuits.Count);
        }

        // Then unsubscribe one
        dummy.Unsubscribe();
        pursuits = ServiceProvider.Get<PursuitDetectionService>().GetSubscribedTechniques();

        if (pursuits.Count != 1)
        {
            Assert.Fail("Assert Fail: The pursuit technique could not unsubscribe properly. Subscribed pursuits: {0}", pursuits.Count);
        }
    }
        
    /// <summary>
    /// Unit Test Case for Update-method. 
    /// Condition: A technique is subscribed to the PursuitDetectionService 
    /// and a gaze point is received which doesn't point on the technique's object.
    /// Result: No pursuit for the object is started.
    /// </summary>
    [Test]
    public void UpdateWithTechniqueWithoutPursuit()
    {
        this.pursuitObject1.transform.position = new Vector3(3, 2, 5);
        DummyPursuit dummy = this.pursuitObject1.AddComponent<DummyPursuit>();
        dummy.InvertPath = true;
        dummy.Subscribe();

        // Check first if it's subscribed correctly
        var pursuits = ServiceProvider.Get<PursuitDetectionService>().GetSubscribedTechniques();

        if (pursuits == null)
        {
            Assert.Fail("Assert Fail: PursuitsDetectionService got none subscribed pursuits, which should not happen.");
        }

        if (pursuits.Count != 1)
        {
            Assert.Fail("Assert Fail: PursuitsDetectionService got {0} instead of 1 subscribed pursuit, which should not happen.", pursuits.Count);
        }

        for (int i = 0; i < 100; i++)
        {
            ServiceProvider.Get<PursuitDetectionService>().FixedUpdate();
            dummy.Move();
        }

        // Check if nothing changed since object is not tracked
        Assert.True(
            dummy.IsUpdated(),
            "OnPursuitUpdated is not called though it should be");
        Assert.False(
            dummy.IsDone(),
            "OnPursuit is called though it should not be");
    }

    /// <summary>
    /// Unit Test Case for Update-method. 
    /// Condition: A technique is subscribed to the PursuitDetectionService 
    /// and gaze points are received which correlate to the object's position.
    /// But those are too few to trigger th interaction.
    /// Result: No pursuit for the object is started.
    /// </summary>
    [Test]
    public void UpdateWithTechniqueTooFewFrames()
    {
        this.pursuitObject1.transform.position = new Vector3(3, 2, 5);
        DummyPursuit dummy = this.pursuitObject1.AddComponent<DummyPursuit>();
        dummy.Subscribe(2000);

        // Check first if it's subscribed correctly
        var pursuits = ServiceProvider.Get<PursuitDetectionService>().GetSubscribedTechniques();

        if (pursuits == null)
        {
            Assert.Fail("Assert Fail: PursuitsDetectionService got none subscribed pursuits, which should not happen.");
        }

        if (pursuits.Count != 1)
        {
            Assert.Fail("Assert Fail: PursuitsDetectionService got {0} instead of 1 subscribed pursuit, which should not happen.", pursuits.Count);
        }

        for (int i = 0; i < 50; i++)
        {
            ServiceProvider.Get<PursuitDetectionService>().FixedUpdate();
            dummy.Move();
        }

        // Check if nothing changed since object is not tracked
        Assert.True(
            dummy.IsUpdated(),
            "OnPursuitUpdated is not called though it should be");
        Assert.False(
            dummy.IsDone(),
            "OnPursuit is called though it should not be");
    }

    /// <summary>
    /// Unit Test Case for Update-method. 
    /// Condition: A technique is subscribed to the PursuitDetectionService 
    /// and gaze points are received which correlate to the object's position.
    /// Result: A pursuit for the object is started.
    /// </summary>
    [Test]
    public void UpdateWithOneTechniqueAndPursuit()
    {
        DummyPursuit dummy = this.pursuitObject1.AddComponent<DummyPursuit>();
        dummy.Subscribe();

        // Check first if it's subscribed correctly
        var pursuits = ServiceProvider.Get<PursuitDetectionService>().GetSubscribedTechniques();

        if (pursuits == null)
        {
            Assert.Fail("Assert Fail: PursuitsDetectionService got none subscribed pursuits, which should not happen.");
        }

        if (pursuits.Count != 1)
        {
            Assert.Fail("Assert Fail: PursuitsDetectionService got {0} instead of 1 subscribed pursuit, which should not happen.", pursuits.Count);
        }

        for (int i = 0; i < 100; i++)
        {
            dummy.Move();
            ServiceProvider.Get<PursuitDetectionService>().FixedUpdate();          
        }

        // Check if the pursuit is started for the object
        Assert.True(
            dummy.IsUpdated(),
            "OnPursuitUpdated is not called though it should be");
        Assert.True(
            dummy.IsDone(),
            "OnPursuit is not called though it should be");
    }

    /// <summary>
    /// Unit Test Case for Update-method. 
    /// Condition: A technique is subscribed to the PursuitDetectionService 
    /// and gaze points are received which correlate to the object's position.
    /// Result: A pursuit for the object is started.
    /// </summary>
    [Test]
    public void UpdateWithOneTechniqueAndPursuitGazeOffset()
    {
        DummyPursuit dummy = this.pursuitObject1.AddComponent<DummyPursuit>();
        dummy.Subscribe();

        // Check first if it's subscribed correctly
        var pursuits = ServiceProvider.Get<PursuitDetectionService>().GetSubscribedTechniques();

        if (pursuits == null)
        {
            Assert.Fail("Assert Fail: PursuitsDetectionService got none subscribed pursuits, which should not happen.");
        }

        if (pursuits.Count != 1)
        {
            Assert.Fail("Assert Fail: PursuitsDetectionService got {0} instead of 1 subscribed pursuit, which should not happen.", pursuits.Count);
        }

        for (int i = 0; i < 100; i++)
        {
            dummy.Move(5);
            ServiceProvider.Get<PursuitDetectionService>().FixedUpdate();
        }

        // Check if the pursuit is started for the object
        Assert.True(
            dummy.IsUpdated(),
            "OnPursuitUpdated is not called though it should be");
        Assert.True(
            dummy.IsDone(),
            "OnPursuit is not called though it should be");
    }

    /// <summary>
    /// Unit Test Case for Update-method. 
    /// Condition: Two techniques are subscribed to the PursuitDetectionService 
    /// and gaze points are received which correlate to the one the object's positions.
    /// Result: A pursuit for the object is started.
    /// </summary>
    [Test]
    public void UpdateWithTwoTechniquesAndOnePursuit()
    {
        DummyPursuit dummy = this.pursuitObject1.AddComponent<DummyPursuit>();
        dummy.Subscribe();

        DummyPursuit dummy2 = this.pursuitObject2.AddComponent<DummyPursuit>();
        dummy2.InvertPath = true;
        dummy2.Subscribe();

        // Check first if it's subscribed correctly
        var pursuits = ServiceProvider.Get<PursuitDetectionService>().GetSubscribedTechniques();

        if (pursuits == null)
        {
            Assert.Fail("Assert Fail: PursuitsDetectionService got none subscribed pursuits, which should not happen.");
        }

        if (pursuits.Count != 2)
        {
            Assert.Fail("Assert Fail: PursuitsDetectionService got {0} instead of 2 subscribed pursuit, which should not happen.", pursuits.Count);
        }

        for (int i = 0; i < 100; i++)
        {
            dummy.Move();
            dummy2.Move();
            ServiceProvider.Get<PursuitDetectionService>().FixedUpdate();
        }

        // Check if the pursuit is started for the objects
        Assert.True(
            dummy.IsUpdated(),
            "OnPursuitUpdated is not called though it should be");
        Assert.True(
            dummy.IsDone(),
            "OnPursuit is not called though it should be");

        Assert.True(
            dummy2.IsUpdated(),
            "OnPursuitUpdated is not called though it should be");
        Assert.False(
            dummy2.IsDone(),
            "OnPursuit is called though it should not be");
    }

    /// <summary>
    /// Unit Test Case for Update-method. 
    /// Condition: Two techniques are subscribed to the PursuitDetectionService 
    /// and gaze points are received which correlate to the one the object's positions.
    /// Result: A pursuit for the object is started.
    /// </summary>
    [Test]
    public void UpdateWithTwoTechniquesAndOnePursuitOneAhead()
    {
        DummyPursuit dummy = this.pursuitObject1.AddComponent<DummyPursuit>();
        dummy.Subscribe();

        DummyPursuit dummy2 = this.pursuitObject2.AddComponent<DummyPursuit>();
        dummy2.SetCounter(20);
        dummy2.Subscribe();

        // Check first if it's subscribed correctly
        var pursuits = ServiceProvider.Get<PursuitDetectionService>().GetSubscribedTechniques();

        if (pursuits == null)
        {
            Assert.Fail("Assert Fail: PursuitsDetectionService got none subscribed pursuits, which should not happen.");
        }

        if (pursuits.Count != 2)
        {
            Assert.Fail("Assert Fail: PursuitsDetectionService got {0} instead of 2 subscribed pursuit, which should not happen.", pursuits.Count);
        }

        for (int i = 0; i < 100; i++)
        {
            dummy.Move();
            dummy2.Move();
            ServiceProvider.Get<PursuitDetectionService>().FixedUpdate();
        }

        // Check if the pursuit is started for the objects
        Assert.True(
            dummy.IsUpdated(),
            "OnPursuitUpdated is not called though it should be");
        Assert.True(
            dummy.IsDone(),
            "OnPursuit is not called though it should be");

        Assert.True(
            dummy2.IsUpdated(),
            "OnPursuitUpdated is not called though it should be");
        Assert.False(
            dummy2.IsDone(),
            "OnPursuit is called though it should not be");
    }

    /// <summary>
    /// Unit Test Case for Update-method. 
    /// Condition: Two techniques are subscribed to the PursuitDetectionService 
    /// and gaze points are received which correlate to the one the object's positions.
    /// Result: A pursuit for the object is started.
    /// </summary>
    [Test]
    public void UpdateWithTwoTechniquesAndTwoPursuits()
    {
        DummyPursuit dummy = this.pursuitObject1.AddComponent<DummyPursuit>();
        dummy.Subscribe();

        DummyPursuit dummy2 = this.pursuitObject2.AddComponent<DummyPursuit>();
        dummy2.Subscribe();

        // Check first if it's subscribed correctly
        var pursuits = ServiceProvider.Get<PursuitDetectionService>().GetSubscribedTechniques();

        if (pursuits == null)
        {
            Assert.Fail("Assert Fail: PursuitsDetectionService got none subscribed pursuits, which should not happen.");
        }

        if (pursuits.Count != 2)
        {
            Assert.Fail("Assert Fail: PursuitsDetectionService got {0} instead of 2 subscribed pursuit, which should not happen.", pursuits.Count);
        }

        for (int i = 0; i < 100; i++)
        {
            dummy.Move();
            dummy2.Move();
            ServiceProvider.Get<PursuitDetectionService>().FixedUpdate();
        }

        // Check if the pursuit is started for the objects
        Assert.True(
            dummy.IsUpdated(),
            "OnPursuitUpdated is not called though it should be");
        Assert.True(
            dummy.IsDone(),
            "OnPursuit is not called though it should be");

        Assert.True(
            dummy2.IsUpdated(),
            "OnPursuitUpdated is not called though it should be");
        Assert.True(
            dummy2.IsDone(),
            "OnPursuit is not called though it should be");
    }

    // ----------------------- Dummy Classes --------------------------------

    /// <summary>
    /// The dummy pupil listener for the PursuitDetectionService test cases.
    /// </summary>
    public class DummyPupilListener : PupilListener
    {
        private readonly Vector2[] positions =
            {
                new Vector2(10, 0), new Vector2(9, 0), new Vector2(8, 0), new Vector2(7, 0), new Vector2(6, 0),
                new Vector2(5, 0), new Vector2(4, 0), new Vector2(3, 0), new Vector2(2, 0), new Vector2(1, 0),
                new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 2), new Vector2(0, 3), new Vector2(0, 4),
                new Vector2(0, 5), new Vector2(0, 6), new Vector2(0, 7), new Vector2(0, 8), new Vector2(0, 9),
                new Vector2(0, 10), new Vector2(1, 10), new Vector2(2, 10), new Vector2(3, 10), new Vector2(4, 10),
                new Vector2(5, 10), new Vector2(6, 10), new Vector2(7, 10), new Vector2(8, 10), new Vector2(9, 10),
                new Vector2(10, 10), new Vector2(10, 9), new Vector2(10, 8), new Vector2(10, 7), new Vector2(10, 6),
                new Vector2(10, 5), new Vector2(10, 4), new Vector2(10, 3), new Vector2(10, 2), new Vector2(10, 1)
            };

        private int counter;

        /// <summary>
        /// Overrides the method from <see cref="PupilListener"/> to provide data usable in Unit Tests.
        /// </summary>
        /// <param name="filtered">
        /// Whether or not the data should be returned.
        /// </param>
        /// <returns>
        /// Gaze coordinates according to the set counter.
        /// </returns>
        public override Vector2 GetCoordinates(bool filtered = true)
        {
            var cam = ServiceProvider.Get<Camera>();
            var v = this.positions[this.counter];
            this.counter = (this.counter + 1) % this.positions.Length;
            return cam.WorldToScreenPoint(new Vector3(v.x, v.y, 4));
        }

        /// <summary>
        /// Sets the counter.
        /// </summary>
        /// <param name="i">
        /// The value the counter is set to.
        /// </param>
        public void SetCounter(int i = 0)
        {
            this.counter = 0;
        }
    }

    /// <summary>
    /// A dummy technique, it's used to test some methods of PursuitDetectionService.
    /// </summary>
    public class DummyPursuit : MonoBehaviour, IPursuitInteractionTechnique
    {
        public bool InvertPath;

        private readonly Vector2[] positions =
            {
                new Vector2(10, 0), new Vector2(9, 0), new Vector2(8, 0), new Vector2(7, 0), new Vector2(6, 0),
                new Vector2(5, 0), new Vector2(4, 0), new Vector2(3, 0), new Vector2(2, 0), new Vector2(1, 0),
                new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 2), new Vector2(0, 3), new Vector2(0, 4),
                new Vector2(0, 5), new Vector2(0, 6), new Vector2(0, 7), new Vector2(0, 8), new Vector2(0, 9),
                new Vector2(0, 10), new Vector2(1, 10), new Vector2(2, 10), new Vector2(3, 10), new Vector2(4, 10),
                new Vector2(5, 10), new Vector2(6, 10), new Vector2(7, 10), new Vector2(8, 10), new Vector2(9, 10),
                new Vector2(10, 10), new Vector2(10, 9), new Vector2(10, 8), new Vector2(10, 7), new Vector2(10, 6),
                new Vector2(10, 5), new Vector2(10, 4), new Vector2(10, 3), new Vector2(10, 2), new Vector2(10, 1)
            };

        private bool done;

        private bool updated;

        private int counter;

        /// <summary>
        /// Moves the object along the path outlined in the positions array.
        /// </summary>
        /// <param name="offset">
        /// The offset on the x-axis.
        /// </param>
        public void Move(float offset = 0)
        {
            var sign = this.InvertPath ? -1 : 1;
            var v = this.positions[this.counter] * sign;
            this.transform.position = new Vector3(v.x + offset, v.y, 4);
            this.counter = (this.counter + 1) % this.positions.Length;
        }

        /// <summary>
        /// Sets the counter.
        /// </summary>
        /// <param name="i">
        /// The value the counter is set to.
        /// </param>
        public void SetCounter(int i)
        {
            this.counter = i;
        }

        /// <summary>
        /// Initialize the values and subscribe to the PursuitDetectionService.
        /// </summary>
        /// <param name="time">
        /// The time used to subscribe it self to the <see cref="PursuitDetectionService"/>.
        /// </param>
        public void Subscribe(int time = 1000)
        {
            // set the variables to false
            this.done = false;
            this.updated = false;

            ServiceProvider.Get<PursuitDetectionService>().Subscribe(this, time);
        }

        /// <summary>
        /// Unsubscribe from the PursuitDetectionService.
        /// </summary>
        public void Unsubscribe()
        {
            ServiceProvider.Get<PursuitDetectionService>().Unsubscribe(this);
        }

        /// <summary>
        /// The on pursuit dropout.
        /// </summary>
        public void OnPursuit()
        {
            this.done = true;
        }

        /// <summary>
        /// The on pursuit update.
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        public void OnPursuitUpdate(PursuitInfo info)
        {
            this.updated = true;
        }

        /// <summary>
        /// Check if the pursuit is ended.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsDone()
        {
            return this.done;
        }

        /// <summary>
        /// Check if the pursuit is continued
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsUpdated()
        {
            return this.updated;
        }
    }
}
