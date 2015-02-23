using UnityEngine;
using System.Collections;

public class MusicSample : MonoBehaviour {

    public float width = 0.1f;
    public float height = 0.5f;
    
    private int[] mainBands = { 50, 94, 176, 331, 620, 1200, 2200, 4100, 7700, 14000, 22000 };
    public int bands = 10;
    public string fileToPlay;

	// Use this for initialization
	void Start () {
        if (fileToPlay == null || fileToPlay.Length == 0) return;

        var music = MusicManager.Instance;

        //music.Play("D:\\Music\\GReeeeN - キセキ.mp3");
        music.Play(fileToPlay);
        //music.Play("E:\\tone2.mp3");

        // Generate blocks.
        var blocks = new SampleBar[bands];
        int fPerBand = bands / (mainBands.Length - 1);

        int startFrequency = 0;
        int endFrequency = 0;
        float max = 24000;
        float slope = 6;
        float ratio = Mathf.Pow(max, 1/slope) / bands;

        for (int i = 0; i < bands; i++)
        {
            startFrequency = endFrequency;
            endFrequency = Mathf.Max(startFrequency+1, (int)((Mathf.Pow((i + 1) * ratio + 0.5f, slope) + 50) / music.BinSize));

            blocks[i] = GameObject.CreatePrimitive(PrimitiveType.Cube).AddComponent<SampleBar>();
            blocks[i].height = height;
            blocks[i].frequency = i;
            blocks[i].transform.parent = transform;
            blocks[i].transform.localScale = new Vector3(
                width, width, width
            );
            blocks[i].transform.localPosition = new Vector3(i * width - (width * bands) / 2, 0, 0);

            blocks[i].startFrequency = startFrequency;
            blocks[i].endFrequency = endFrequency;

        }

        //for (int i = 0; i < bands; i++)
        //{
        //    float progress = (float)(i % fPerBand) / fPerBand;
        //    float progress2 = (float)(i % fPerBand + 1) / fPerBand;

        //    //int low = mainBands[i / fPerBand];
        //    int nextIndex = Mathf.Min((i / fPerBand) + 1, mainBands.Length - 1);
        //    int startFrequency = mainBands[i / fPerBand] / music.BinSize;
        //    int endFrequency = mainBands[nextIndex] / music.BinSize;
        //    int fWidth = endFrequency - startFrequency;
        //    int step = fWidth / fPerBand;

        //    blocks[i] = GameObject.CreatePrimitive(PrimitiveType.Cube).AddComponent<SampleBar>();
        //    blocks[i].height = height;
        //    blocks[i].frequency = i;
        //    blocks[i].transform.parent = transform;
        //    blocks[i].transform.localScale = new Vector3(
        //        width, width, width    
        //    );
        //    blocks[i].transform.localPosition = new Vector3(i * width - (width * bands) / 2, 0, 0);

        //    blocks[i].startFrequency = (int)(startFrequency + progress * fWidth);
        //    blocks[i].endFrequency = (int)(startFrequency + progress2 * fWidth);


        //}

	}

    // Update is called once per frame
    void Update()
    {
	    
	}
}
