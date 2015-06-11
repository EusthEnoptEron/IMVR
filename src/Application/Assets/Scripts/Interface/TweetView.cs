using UnityEngine;
using System.Collections;
using LinqToTwitter;
using UnityEngine.UI;
using DG.Tweening;

public class TweetView : MonoBehaviour {
    public Status tweet;

    public Text textNode;
    public Text titleNode;
    public float speed = 0.5f;
    public float lifetime = 5f;

    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        _canvasGroup.alpha = 0;
    }

	// Use this for initialization
	void Start () {
        if (tweet != null)
        {
            textNode.text = tweet.Text;
            titleNode.text = tweet.User.UserID;

            StartCoroutine(TweetLifecycle());
        }
        else
        {
            Debug.LogError("Invalid tweet!");
        }
	}

    void Update()
    {
        // Move constantly upward
        var gazeDirection = Camera.main.transform.forward;
        var direction = (transform.position - Camera.main.transform.position).normalized;
        var dotProduct = Vector3.Dot(gazeDirection, direction);

        if (dotProduct > 0.95f)
        {
            transform.position = Vector3.Lerp(transform.position, Camera.main.transform.position + direction * 1f, Time.deltaTime * 2 * dotProduct);
        }
        else
        {
           transform.position += Vector3.up * Time.deltaTime * speed;
        }
        transform.rotation = Quaternion.LookRotation( (transform.position - Camera.main.transform.position).normalized, Vector3.up );

    }

	// Update is called once per frame
	IEnumerator TweetLifecycle () {
	    // Appear!
        yield return _canvasGroup.Fade(1, 1).WaitForCompletion();

        // Stay for a while
        yield return new WaitForSeconds(lifetime);

        // Disappear
        yield return _canvasGroup.Fade(0, 1).WaitForCompletion();
        GameObject.Destroy(gameObject);
	}
}
