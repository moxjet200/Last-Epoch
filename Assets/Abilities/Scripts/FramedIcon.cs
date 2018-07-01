using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FramedIcon : MonoBehaviour {
    
    Image image = null;
    Image frame = null;

	// Use this for initialization
	void Awake () {
        GameObject go = new GameObject();
        go.transform.parent = transform;
        RectTransform rect = go.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(0.5f, 0.5f);
        float sizeScaleX = (float)rect.sizeDelta.x / (float) 54f;
        float sizeScaleY = (float)rect.sizeDelta.y / (float) 54f;
        //go.transform.position = new Vector2(, rect.sizeDelta/54f)
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setSprite(Sprite sprite)
    {
        image.sprite = sprite;
    }

    


}
