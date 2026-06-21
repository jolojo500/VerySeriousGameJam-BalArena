using UnityEditor.PackageManager;
using UnityEngine;
using Venice;

public class EnemyBallerina : MonoBehaviour
{
    private Rigidbody _rb;
    public EnemyVisual Visual;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnHit(HitInfo info)
    {
        Debug.Log("Ouch, I'm a ballerina!");

        Vector3 dir = (transform.position - info.SourcePosition).normalized;
        dir.y = 0f;
        if (dir != Vector3.zero)
            _rb.AddForce(dir.normalized * info.KnockbackForce, ForceMode.Impulse);

        _rb.AddForce(Vector3.up * info.KnockbackForce, ForceMode.Impulse);
        Visual?.ApplySquashAndStretch(1.1f, .2f);
    }
}
