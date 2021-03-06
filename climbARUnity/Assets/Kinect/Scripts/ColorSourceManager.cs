﻿using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class ColorSourceManager : MonoBehaviour
{
    public int ColorWidth { get; private set; }
    public int ColorHeight { get; private set; }

    private KinectSensor _Sensor;
    private ColorFrameReader _Reader;
    private Texture2D _Texture;
    private byte[] _Data;
    private Color32[] resetColorArray;

    private bool hasStateManager = true;

    public Texture2D GetColorTexture()
    {
        return _Texture;
    }

    void Start()
    {
        if (StateManager.instance)
        {
            //StateManager.instance.debugView = true;
        }
        else
        {
            this.hasStateManager = false;
        }

        _Sensor = KinectSensor.GetDefault();

        if (_Sensor != null)
        {
            _Reader = _Sensor.ColorFrameSource.OpenReader();

            var frameDesc = _Sensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Rgba);
            ColorWidth = frameDesc.Width;
            ColorHeight = frameDesc.Height;

            _Texture = new Texture2D(frameDesc.Width, frameDesc.Height, TextureFormat.RGBA32, false);
            _Data = new byte[frameDesc.BytesPerPixel * frameDesc.LengthInPixels];

            Color32 resetColor = new Color32(0, 0, 0, 0);
            resetColorArray = _Texture.GetPixels32();
            for (int i = 0; i < resetColorArray.Length; i++)
            {
                resetColorArray[i] = resetColor;
            }

            if (!_Sensor.IsOpen)
            {
                _Sensor.Open();
            }
        }
        else
        {
            Debug.Log("cannot get Kinect sensor");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown("t"))
        {
            StateManager.instance.debugView = 
                !StateManager.instance.debugView;
        }
        if (this.hasStateManager && !StateManager.instance.debugView)
        {
            _Texture.SetPixels32(resetColorArray);
            _Texture.Apply();
        }
        else if (_Reader != null)
        {
            var frame = _Reader.AcquireLatestFrame();

            if (frame != null)
            {
                frame.CopyConvertedFrameDataToArray(
                    _Data,
                    ColorImageFormat.Rgba);
                _Texture.LoadRawTextureData(_Data);
                _Texture.Apply();

                frame.Dispose();
                frame = null;
            } 
        }
        else
        {
            Debug.Log("cannot get Kinect reader");
        }
    }

    void OnApplicationQuit()
    {
        if (_Reader != null)
        {
            _Reader.Dispose();
            _Reader = null;
        }

        if (_Sensor != null)
        {
            if (_Sensor.IsOpen)
            {
                _Sensor.Close();
            }

            _Sensor = null;
        }
    }
}
