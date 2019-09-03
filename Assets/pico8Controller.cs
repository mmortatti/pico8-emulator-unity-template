using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pico8_Emulator;
using UnityEngine.UI;

public class pico8Controller : MonoBehaviour
{
    public RawImage rawImage;

    private Color[] screenColorData;
    Pico8<Color> pico8;
    Texture2D screenTexture;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 30;

        pico8 = new Pico8<Color>();
        screenColorData = new Color[128 * 128];
        screenTexture = new Texture2D(128, 128, TextureFormat.RGBA32, false, true);
        screenTexture.filterMode = FilterMode.Point;

        pico8.screenColorData = screenColorData;
        pico8.rgbToColor = (r, g, b) => new Color(r / 255.0f, g / 255.0f, b / 255.0f);
        pico8.AddLeftButtonDownFunction(() => { return Input.GetKey(KeyCode.LeftArrow); }, 0);
        pico8.AddRightButtonDownFunction(() => { return Input.GetKey(KeyCode.RightArrow); }, 0);
        pico8.AddUpButtonDownFunction(() => { return Input.GetKey(KeyCode.UpArrow); }, 0);
        pico8.AddDownButtonDownFunction(() => { return Input.GetKey(KeyCode.DownArrow); }, 0);
        pico8.AddOButtonDownFunction(() => { return Input.GetKey(KeyCode.Z); }, 0);
        pico8.AddXButtonDownFunction(() => { return Input.GetKey(KeyCode.X); }, 0);

        pico8.audio.sampleRate = AudioSettings.outputSampleRate;

        pico8.LoadGame("Assets/jelpi.p8", new NLuaInterpreter());

        rawImage.texture = screenTexture;
    }

    // Update is called once per frame
    void Update()
    {
        pico8.Draw();

        screenTexture.SetPixels(screenColorData);
        screenTexture.Apply();
    }

    void FixedUpdate()
    {
        pico8.Update();
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        int dataLen = data.Length / channels;
        float[] buffer = pico8.audio.RequestBuffer(dataLen);
        for(int i = 0; i < dataLen; i += 1)
        {
            for (int j = 0; j < channels; j += 1)
                data[i * channels + j] = buffer[i];
        }
    }
}
