
using UnityEngine;

public class ParticleDropBehavior : MonoBehaviour
{

    private Rigidbody rigid;
    private Vector3 dropVector;

    // Use this for initialization
    public void Start()
    {
        this.rigid = gameObject.GetComponent<Rigidbody>();

        // Calculate a random force vector for this particle.
        int x = Random.Range(-50, 50);
        int y = Random.Range(-60, -100);
        int z = Random.Range(-50, 50);

        this.dropVector = new Vector3(x, y, z);

        // Destroy the object after a random time.
        GameObject.Destroy(this.gameObject, Random.Range(0.5f, 1.5f));
    }

    // Update is called once per frame
    public void Update()
    {
        rigid.AddForce(this.dropVector);
    }
}
