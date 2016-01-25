using UnityEngine;
using System.Collections;
using Windows.Kinect;
using System.IO;

public class ColorView : MonoBehaviour {


    public GameObject colorMyMang;
    private colorMyMang _ColorManager;
    private Texture2D ttt;

    public GameObject cubeRR;

	// Use this for initialization
	void Start () {
        gameObject.GetComponent<Renderer>().material.SetTextureScale("_MainTex", new Vector2(-1, 1));

	}
	
	// Update is called once per frame
	void Update () {
        if (colorMyMang == null)
        {
            return;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log(Input.mousePosition);

            _ColorManager = colorMyMang.GetComponent<colorMyMang>();
            if(_ColorManager == null)
            {
                return;
            }

            ttt = _ColorManager.GetColorTexture();
            Texture2D result = new Texture2D((int)1024, (int)1024);
            result.SetPixels(ttt.GetPixels(Mathf.FloorToInt(448),Mathf.FloorToInt(28),Mathf.FloorToInt(1024),Mathf.FloorToInt(1024)));
            //TextureScale.Bilinear(result, 32, 32);
            TextureScale.Bilinear(result, 32, 32);
            


            byte[] bytes = result.EncodeToPNG();
            File.WriteAllBytes("SavedScreen.png", bytes);
            cubeRR.GetComponent<Renderer>().material.mainTexture = ttt;

        }
	}



}
