//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//
//public class HealthBar : MonoBehaviour
//{
//    public GameObject enemy;
//    public Transform foregroundTransform;
//    private float originalScale;
//
//    // Start is called before the first frame update
//    void Start()
//    {
//        originalScale = foregroundTransform.localScale.x;
//    }
//
//    // Update is called once per frame
//    void Update()
//    {
//        // Point the health bar towards the camera
//        transform.LookAt(Camera.main.transform.position, Vector3.up);
//
//        Enemy enemyScript = enemy.GetComponent<Enemy>();
//        float healthPercent = (float)enemyScript.CurrentHp / enemyScript.MaxHp;
//
//        // Update the health bar scale
//        Vector3 scale = foregroundTransform.localScale;
//        scale.x = healthPercent * originalScale;
//        foregroundTransform.localScale = scale;
//    }
//}
//