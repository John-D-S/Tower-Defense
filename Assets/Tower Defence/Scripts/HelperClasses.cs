using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperClasses : MonoBehaviour
{
    public static class WeaponFunctions
    {
        public static Quaternion RandomSpread(float _spread)
        {
            float randomXSpread = Random.Range(-_spread, _spread) * 0.5f;
            float randomYSpread = Random.Range(-_spread, _spread) * 0.5f;
            return Quaternion.Euler(randomXSpread, randomYSpread, 0);
        }

        public static IEnumerator ShootBullet(Collider spawningCollider, Vector3 _startPosition, Quaternion _direction, float _bulletSpeed, GameObject _bullet, float _bulletRadius, float _bulletDamage, float _spread, float _range, string _targetTag)
        {
            GameObject bulletInstance = Instantiate(_bullet, _startPosition, _direction * RandomSpread(_spread));
            bulletInstance.transform.localScale = Vector3.one * _bulletRadius * 2;
            float distance = 0;
            while (distance < _range)
            {
                yield return new WaitForFixedUpdate();
                float distanceDelta = Time.deltaTime * _bulletSpeed;
                RaycastHit hitInfo;
                if (Physics.SphereCast(new Ray(bulletInstance.transform.position, bulletInstance.transform.forward), _bulletRadius, out hitInfo, distanceDelta))
                {
                    //if the bullet hits the thing that it's supposed to, damage the thing
                    if (hitInfo.collider.tag == _targetTag)
                    {
                        IKillable objectToDamage = hitInfo.collider.GetComponentInParent<IKillable>();
                        if (objectToDamage != null)
                            objectToDamage.Damage(_bulletDamage);
                    }
                    else if (hitInfo.collider != spawningCollider)
                    {
                        break;
                    }
                }
                bulletInstance.transform.position += bulletInstance.transform.forward * distanceDelta;
                distance += distanceDelta;
            }
            Destroy(bulletInstance);
        }
    }

    public static class HelperFunctions
    {
        #region MouseRayHitPoint function and overloads
        /// <summary>
        /// Returns the point in space where the cursor is hovering over a collider.
        /// </summary>
        public static Vector3 MouseRayHitPoint()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                return hit.point;
            }
            return MouseRayHitPoint(0f);
        }

        /// <summary>
        /// Returns the point in space where the cursor is hovering over a collider of an object with the specified layer. 
        /// </summary>
        /// <param name="_layerMask">The layer of the object you want to raycast to.</param>
        public static Vector3 MouseRayHitPoint(LayerMask _layerMask)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _layerMask))
            {
                return hit.point;
            }
            return MouseRayHitPoint(0f);
        }

        /// <summary>
        /// Returns the point in space where the cursor is hovering over an infinite, upward facing plane with a specified y height.
        /// </summary>
        /// <param name="TargetHeight">The height in space of the plane.</param>
        public static Vector3 MouseRayHitPoint(float TargetHeight)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, Vector3.up * TargetHeight);
            float distance = 0;
            if (plane.Raycast(ray, out distance))
            {
                return ray.GetPoint(distance);
            }
            else return Vector3.zero;
        }
        #endregion

        /// <summary>
        /// Converts a vector 2 into a point in space on the horizontal plane at y = 0.
        /// </summary>
        public static Vector3 ConvertToVector3(Vector2 input)
        {
            return new Vector3(input.x, 0, input.y);
        }

        public static Vector2 ConvertToVector2(Vector3 input)
        {
            return new Vector2(input.x, input.z);
        }

        /// <summary>
        /// Returns the Y height of colliders of objects with the Terrain layer at the given coordinates on the horizontal plane.
        /// </summary>
        public static float TargetHeight(Vector2 _targetPosition)
        {
            LayerMask terrain = LayerMask.GetMask("Terrain");
            RaycastHit hit;
            if (Physics.Raycast(ConvertToVector3(_targetPosition) + Vector3.up * 1000, Vector3.down, out hit, Mathf.Infinity, terrain))
            {
                return hit.point.y;
            }
            return 0;
        }

        public static Vector3 GetGroundNormal(Vector2 _position)
        {
            LayerMask terrain = LayerMask.GetMask("Terrain");
            RaycastHit hit;
            if (Physics.Raycast(ConvertToVector3(_position) + Vector3.up * 1000, Vector3.down, out hit, Mathf.Infinity, terrain))
            {
                return hit.normal;
            }
            return Vector3.up;
        }

        public static Vector2 Angle2Direction(float _angle)
        {
            return ConvertToVector2(Quaternion.Euler(0, _angle, 0) * Vector3.forward);
        }

        public static float Direction2Angle(Vector2 _direction)
        {
            float angle = Vector3.Angle(new Vector3(0.0f, 1.0f, 0.0f), new Vector3(_direction.x, _direction.y, 0.0f));
            if (_direction.x < 0.0f)
            {
                angle = -angle;
                angle = angle + 360;
            }
            return angle;
        }
    }
}