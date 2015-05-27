using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(CircleLayout))]
public class CylinderRaycaster : BaseRaycaster {
    private const float D3_EPSILON = 0.01f;
    private Camera camera;
    public bool castInChildren = true;
    CircleLayout cylinder;

    public void Start()
    {
        cylinder = GetComponent<CircleLayout>();
        camera = Camera.main;
        //camera = GameObject.FindGameObjectWithTag("ForegroundCamera").GetComponentInChildren<Camera>();
    }


    public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
    {
        var ray = eventCamera.ScreenPointToRay(eventData.position);

        float lambda;
        if (d3RayCylinderIntersection(ray, out lambda))
        {
            //Debug.DrawLine(ray.origin + ray.direction * lambda, ray.origin + ray.direction * (lambda + 0.1f), Color.green);

            //var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //sphere.gameObject.layer = LayerMask.NameToLayer("Foreground");
            //sphere.transform.localScale = Vector3.one * 0.01f;
            //sphere.transform.position = ray.origin + ray.direction * lambda;
            //Destroy(sphere, 0.1f);
            var tile = cylinder.GetTileAtPosition(ray.origin + ray.direction * lambda);
            if (tile == null) return;

            if (castInChildren)
            {
                int before = resultAppendList.Count;
                tile.GetComponentsInChildren<BaseRaycaster>()
                    .Where(raycaster => !raycaster.enabled)
                    .ToList()
                    .ForEach(raycaster => raycaster.Raycast(eventData, resultAppendList));
                if (resultAppendList.Count > before)
                {
                    //Debug.Log("------------");
                    //for (int i = resultAppendList.Count - (resultAppendList.Count - before); i < resultAppendList.Count; i++)
                    //    Debug.LogFormat("{0} ({1})", resultAppendList[i].gameObject.GetPath(), resultAppendList[i].module.gameObject.GetPath());
                    //Debug.Log("____________");
                    return;
                }
            }


            var canvas = tile.GetComponentInParent<Canvas>();

            //Debug.Log(tile.name);

            resultAppendList.Add(new RaycastResult()
            {
                gameObject = tile.gameObject,
                module = this,
                depth = 1,
                distance = lambda,
                index = resultAppendList.Count,
                sortingLayer = canvas ? canvas.sortingLayerID : 0,
                sortingOrder = canvas ? canvas.sortingOrder : -20
            });
        }
    }

    bool d3RayCylinderIntersection(Ray ray, out float lambda/*, out Vector3 normal, out Vector3 newPosition*/)
    // Ray and cylinder intersection
    // If hit, returns true and the intersection point in 'newPosition' with a normal and distance along
    // the ray ('lambda')
    {
        lambda = 0;
       // normal = Vector3.zero;
      Vector3 RC;
      float d;
      float t,s;
      Vector3 n,D,O;
      float ln;
      float @in,@out;

      RC=ray.origin - cylinder.transform.position;
      n = Vector3.Cross(ray.direction, cylinder.transform.up);

      ln= n.magnitude;

      // Parallel? (?)
      if((ln<D3_EPSILON)&&(ln>-D3_EPSILON))
        return false;

      n.Normalize();

      d = Mathf.Abs( Vector3.Dot(RC, n));

      if (d <= cylinder.radius)
      {
          O = Vector3.Cross(RC, cylinder.transform.up);
          t = -Vector3.Dot(O, n)/ln;

        //TVector::cross(n,cylinder._Axis,O);
          O = Vector3.Cross(n, cylinder.transform.up);
          O.Normalize();

        s= Mathf.Abs( Mathf.Sqrt(cylinder.radius*cylinder.radius-d*d) / Vector3.Dot(ray.direction, O) );

        @in=t-s;
        @out=t+s;

        if (@in<-D3_EPSILON)
        {
          if(@out<-D3_EPSILON)
            return false;
          else lambda=@out;
        } else if(@out<-D3_EPSILON)
        {
          lambda=@in;
        } else if(@in<@out)
        {
          lambda=@in;
        } else
        {
          lambda=@out;
        }

        if (lambda < 0) return false;
        // Calculate intersection point
       /* newPosition=ray.origin + ray.direction * lambda;

        Vector3 HB = newPosition - cylinder.transform.position;

        float scale= Vector3.Dot(HB, cylinder.transform.up);
        normal = HB - cylinder.transform.up * scale;
        normal.Normalize();
          */
        return true;
      }

      return false;
    }


    public override Camera eventCamera
    {
        get { return camera; }
    }
}
