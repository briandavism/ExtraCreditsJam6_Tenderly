using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[SerializeField]
//private Vector2 min;
//[SerializeField]
//private Vector2 max;
//[SerializeField]
//private Vector2 yRotationRange;
//[SerializeField]
//[Range(0.01f, 0.1f)]
//private float lerpSpeed = 0.05f;

//private void Awake()
//{
//    _newPosition = transform.position;
//    _newRotation = transform.rotation;
//}

//private void Update()
//{
//    transform.position = Vector3.Lerp(transform.position, _newPosition, Time.deltaTime * lerpSpeed);
//    transform.rotation = Quaternion.Lerp(transform.rotation, _newRotation, Time.deltaTime * lerpSpeed);

//    if(Vector3.Distance(transform.position, _newPosition) <1f)
//    {
//        GetNewPosition();
//    }
//}

//void GetNewPosition()
//{
//    var xPos = Random.Range(min.x, max.x);
//    var xPos = Random.Range(min.y, max.y);
//    _newRoration = Quaternion.Euler(0, Random.Range(yRotationRange.x, yRotationRange.y), 0);
//    _newPosition = new Vector3(xPos, 0, zPos);
//}