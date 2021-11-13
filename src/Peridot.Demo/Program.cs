// Copyright (c) 2021 ezequias2d <ezequiasmoises@gmail.com> and the Peridot contributors
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Peridot.Veldrid;
using System.Drawing;
using System.Drawing.Imaging;
using Veldrid;
using Veldrid.StartupUtilities;
using System.Numerics;
using Peridot.Demo;
using Ez.Numerics;

var title = "Peridot.Demo";
var wci = new WindowCreateInfo(100, 100, 640, 480, WindowState.Normal, title);
VeldridStartup.CreateWindowAndGraphicsDevice(wci, out var window, out var gd);

window.Resized += () =>
{
    gd.MainSwapchain.Resize((uint)window.Width, (uint)window.Height);
};

var shaders = VeldridSpriteBatch.LoadDefaultShaders(gd);
var sb = new VeldridSpriteBatch(gd, gd.MainSwapchain.Framebuffer.OutputDescription, shaders);
var cl = gd.ResourceFactory.CreateCommandList();
var fence = gd.ResourceFactory.CreateFence(false);
var texture = LoadTexture(Resource._4_2_07);
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
    //ref var item = ref sb.Draw(texture);
    var size = new Vector2(texture.Width, texture.Height);
    var pos = size * -0.5f;
    var source = new System.Drawing.Rectangle(0, 0, (int)texture.Width, (int)texture.Height);

    //item = new(pos, size, source, Color.White, 0f, Vector2.Zero, Vector2.One, 1);
    sb.Draw(texture, default, source, Color.White, EzMath.Deg2Rad * count / 1000, size * 0.5f, Vector2.One, 1);
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

Texture LoadTexture(Bitmap image)
{

    var texture = gd.ResourceFactory.CreateTexture(new()
    {
        Format = Veldrid.PixelFormat.B8_G8_R8_A8_UNorm,
        Height = (uint)image.Height,
        Width = (uint)image.Width,
        Depth = 1,
        ArrayLayers = 1,
        MipLevels = 1,
        SampleCount = TextureSampleCount.Count1,
        Type = TextureType.Texture2D,
        Usage = TextureUsage.Sampled,
    });
    image.RotateFlip(RotateFlipType.RotateNoneFlipY);
    image = image.Clone(new(0, 0, image.Width, image.Height), System.Drawing.Imaging.PixelFormat.Format32bppArgb);

    var data = image.LockBits(new(default, new(image.Width, image.Height)), ImageLockMode.ReadOnly, image.PixelFormat);
    var size = (uint)(data.Stride * data.Height);
    gd.UpdateTexture(texture, data.Scan0, size, 0, 0, 0, (uint)image.Width, (uint)image.Height, 1, 0, 0);

    image.UnlockBits(data);

    return texture;
}