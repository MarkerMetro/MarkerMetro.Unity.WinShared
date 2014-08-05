using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace Mindscape.Raygun4Unity.Messages
{
  public class RaygunEnvironmentMessage
  {
    public RaygunEnvironmentMessage()
    {
      try
      {
        UtcOffset = -Math.Round((DateTime.UtcNow - DateTime.Now).TotalHours + 0.01, 1);
        
        Locale = Application.systemLanguage.ToString();

        ResolutionWidth = Screen.currentResolution.width;
        ResolutionHeight = Screen.currentResolution.height;
        RefreshRate = Screen.currentResolution.refreshRate;
        ShowCursor = Screen.showCursor;
        FullScreen = Screen.fullScreen;
        Orientation = Screen.orientation.ToString();

        LoadedLevelName = Application.loadedLevelName;

        ProcessorCount = SystemInfo.processorCount;
        Cpu = SystemInfo.processorType;
        OSVersion = SystemInfo.operatingSystem;
        DeviceModel = SystemInfo.deviceModel;
        DeviceType = SystemInfo.deviceType.ToString();
        SystemMemorySize = SystemInfo.systemMemorySize;
        Platform = Application.platform.ToString();

        GraphicsDeviceName = SystemInfo.graphicsDeviceName;
        GraphicsDeviceVendor = SystemInfo.graphicsDeviceVendor;
        GraphicsDeviceVersion = SystemInfo.graphicsDeviceVersion;
        GraphicsMemorySize = SystemInfo.graphicsMemorySize;
        GraphicsShaderLevel = SystemInfo.graphicsShaderLevel;
        MaxTextureSize = SystemInfo.maxTextureSize;
        NpotSupport = SystemInfo.npotSupport.ToString();
        SupportedRenderTargetCount = SystemInfo.supportedRenderTargetCount;
        Supports3DTextures = SystemInfo.supports3DTextures;
        SupportsAccelerometer = SystemInfo.supportsAccelerometer;
        SupportsComputeShaders = SystemInfo.supportsComputeShaders;
        SupportsImageEffects = SystemInfo.supportsImageEffects;
        SupportsGyroscope = SystemInfo.supportsGyroscope;
        SupportsInstancing = SystemInfo.supportsInstancing;
        SupportsLocationService = SystemInfo.supportsLocationService;
        SupportsRenderTextures = SystemInfo.supportsRenderTextures;
        SupportsRenderToCubemap = SystemInfo.supportsRenderToCubemap;
        SupportsShadows = SystemInfo.supportsShadows;
        SupportsSparseTextures = SystemInfo.supportsSparseTextures;
        SupportsStencil = SystemInfo.supportsStencil;
        SupportsVertexPrograms = SystemInfo.supportsVertexPrograms;
        SupportsVibration = SystemInfo.supportsVibration;
      }
      catch (Exception ex)
      {
        RaygunClient.Log(string.Format("Error getting environment info: {0}", ex.Message));
      }
    }

    public int ResolutionWidth { get; set; }

    public int ResolutionHeight { get; set; }

    public int RefreshRate { get; set; }

    public bool ShowCursor { get; set; }

    public bool FullScreen { get; set; }

    public string Orientation { get; set; }

    public string LoadedLevelName { get; set; }

    public int ProcessorCount { get; set; }

    public string Cpu { get; set; }

    public string OSVersion { get; set; }

    public string DeviceModel { get; set; }

    public string DeviceType { get; set; }

    public int SystemMemorySize { get; set; } // MB

    public string Platform { get; set; }

    public string GraphicsDeviceName { get; set; }

    public string GraphicsDeviceVendor { get; set; }

    public string GraphicsDeviceVersion { get; set; }

    public int GraphicsMemorySize { get; set; }

    public int GraphicsShaderLevel { get; set; }

    public int MaxTextureSize { get; set; }

    public string NpotSupport { get; set; }

    public int SupportedRenderTargetCount { get; set; }

    public bool Supports3DTextures { get; set; }

    public bool SupportsAccelerometer { get; set; }

    public bool SupportsComputeShaders { get; set; }

    public bool SupportsGyroscope { get; set; }

    public bool SupportsImageEffects { get; set; }

    public bool SupportsInstancing { get; set; }

    public bool SupportsLocationService { get; set; }

    public bool SupportsRenderTextures { get; set; }

    public bool SupportsRenderToCubemap { get; set; }

    public bool SupportsShadows { get; set; }

    public bool SupportsSparseTextures { get; set; }

    public int SupportsStencil { get; set; }

    public bool SupportsVertexPrograms { get; set; }

    public bool SupportsVibration { get; set; }

    public double UtcOffset { get; set; }

    public string Locale { get; set; }
  }
}
