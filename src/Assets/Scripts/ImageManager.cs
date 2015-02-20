using UnityEngine;
using System.Collections;
using System.Linq;

public class ImageManager : MonoBehaviour {
    ImageSource source;
	// Use this for initialization
	void Start () {
        source = new ImageSource(@"C:\Users\Simon\Pictures");
        StartCoroutine(FillScene());
        //grabber.GrabImages(@"C:\Users\Simon\Pictures");
        //grabber.GrabImages(@"C:");
	}

    IEnumerator FillScene()
    {
        int height = 3;
        int offset = 0;
        int imageCount = 100;
        while (offset < imageCount)
        {
            yield return 0;
            //yield return new WaitForSeconds(1);

            var buffer = source.ReadForward();
            int imagesPerRevolution = imageCount / height;
            float radius = imagesPerRevolution / (Mathf.PI * 2);

            for (int i = 0; i < buffer.Count(); i++)
            {
                Vector2 pos = new Vector2(((offset + i) / height) * (360f / imagesPerRevolution) * Mathf.PI / 180f, (offset + i) % height);

                buffer[i].transform.localPosition = new Vector3(Mathf.Cos(pos.x) * radius, pos.y, Mathf.Sin(pos.x) * radius);
                buffer[i].transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(-buffer[i].transform.position, Vector3.up).normalized); 

            }
            offset += buffer.Count();
        }
    }

	// Update is called once per frame
	void Update () {
	}

    void OnApplicationQuit()
    {
        source.Dispose();
    }

}
