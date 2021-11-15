// Copyright (c) 2021 ezequias2d <ezequiasmoises@gmail.com> and the Peridot contributors
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Peridot.Veldrid;
using System.Drawing;
using Veldrid;
using Veldrid.StartupUtilities;
using System.Numerics;
using Peridot.Demo;

using Font = Peridot.Veldrid.Font;
using StbImageSharp;
using System.Diagnostics;

var title = "Peridot.Demo";
var wci = new WindowCreateInfo(100, 100, 640, 480, WindowState.Normal, title);

var window = VeldridStartup.CreateWindow(wci);
var gd = VeldridStartup.CreateVulkanGraphicsDevice(
    new(true,
        Veldrid.PixelFormat.D24_UNorm_S8_UInt,
        true, 
        ResourceBindingModel.Default,
        true,
        true),
    window);

window.Resized += () =>
{
    gd.MainSwapchain.Resize((uint)window.Width, (uint)window.Height);
};

var shaders = VeldridSpriteBatch.LoadDefaultShaders(gd);
var sb = new VeldridSpriteBatch(gd, gd.MainSwapchain.Framebuffer.OutputDescription, shaders);
var tr = new TextRenderer(gd, sb);
var cl = gd.ResourceFactory.CreateCommandList();
var fence = gd.ResourceFactory.CreateFence(false);
var texture = LoadTexture(Resource._4_2_07);
var font = LoadFont(Resource.romulus);
var count = 0;

var time = DateTime.Now;
while (window.Exists)
{
    count++;

    var now = DateTime.Now;
    var delta = now - time;
    var deltaTime = delta.TotalSeconds;
    time = now;

    if((count % 1000) == 0)
    {
        window.Title = title + " " + (1f / deltaTime);
    }

    window.PumpEvents();

    sb.Begin();
    sb.ViewMatrix = Matrix4x4.CreateOrthographic(window.Width, window.Height, 0.01f, 1000f);
    
    var size = new Vector2(texture.Width, texture.Height);
    var pos = size * -0.5f;
    var source = new System.Drawing.Rectangle(0, 0, (int)texture.Width / 2, (int)texture.Height / 2);

    sb.Draw(texture, default, source, Color.White, 0, size * 0.5f, Vector2.One, 1);
    tr.DrawString(font, 32, "Hello World!", new Vector2(1, 1), Color.White, 0, new Vector2(0, 0), new Vector2(1), 1);
    sb.End();

    cl.Begin();
    cl.SetFramebuffer(gd.SwapchainFramebuffer);
    cl.ClearColorTarget(0, RgbaFloat.CornflowerBlue);
    sb.DrawBatch(cl);
    cl.End();

    fence.Reset();
    gd.SubmitCommands(cl, fence);
    gd.WaitForFence(fence);
    gd.SwapBuffers();
}

Texture LoadTexture(byte[] data)
{
    ImageResult image;
    {
        image = ImageResult.FromMemory(data, ColorComponents.Default);
        Debug.Assert(image != null);
    }

    var td = new TextureDescription(
        (uint)image.Width, (uint)image.Height, 1,
        1, 1,
        PixelFormat.R8_G8_B8_A8_UNorm,
        TextureUsage.Sampled,
        TextureType.Texture2D);

    var texture = gd.ResourceFactory.CreateTexture(td);
    gd.UpdateTexture(texture, image.Data, 0, 0, 0, td.Width, td.Height, td.Depth, 0, 0);

    return texture;
}

Font LoadFont(byte[] data)
{
    using var stream = new MemoryStream();
    stream.Write(data);
    stream.Position = 0;
    var font = new Font();
    font.AddFont(data);
    return font;
}