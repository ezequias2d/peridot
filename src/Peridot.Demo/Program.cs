// Copyright (c) 2021 ezequias2d <ezequiasmoises@gmail.com> and the Peridot contributors
// This code is licensed under MIT license (see LICENSE for details)

using Peridot.Veldrid;
using System.Drawing;
using Veldrid;
using Veldrid.StartupUtilities;
using System.Numerics;
using Peridot.Demo;

using Font = Peridot.Veldrid.Font;
using StbImageSharp;
using System.Diagnostics;
using Rectangle = System.Drawing.Rectangle;
using Peridot;

var title = "Peridot.Demo";
var wci = new WindowCreateInfo(100, 100, 640, 480, WindowState.Normal, title);

var window = VeldridStartup.CreateWindow(wci);
var gd = VeldridStartup.CreateDefaultD3D11GraphicsDevice(
    new(true,
        Veldrid.PixelFormat.D32_Float_S8_UInt,
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
var neg = false;
var ps = 0;
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
    sb.ViewMatrix = Matrix4x4.CreateOrthographic(window.Width, window.Height, 0.01f, -100f);
    
    var size = new Vector2(texture.Width, texture.Height);
    var pos = size * -0.5f;
    var source = new System.Drawing.Rectangle((int)texture.Width / 2, (int)texture.Height / 2, (int)texture.Width / 2, (int)texture.Height / 2);

    ps += neg ? -1 : 1;
    if (ps == 0 || ps == 100)
        neg = !neg;

    var options = ps >= 0 && ps < 25 ? SpriteOptions.None :
                  ps >= 25 && ps < 50 ? SpriteOptions.FlipVertically :
                  ps >= 50 && ps < 75 ? SpriteOptions.FlipVertically | SpriteOptions.FlipHorizontally :
                                        SpriteOptions.FlipHorizontally;

    sb.Draw(texture, default, source, Color.White, 0, new(-size.X / 6, 0), Vector2.One, options, 1f);

    var s = font.MeasureString("Hello World!", 32);
    var strScissor = new Rectangle(0, 0, (int)s.X, (int)s.Y * ps / 100);
    tr.DrawString(font, 32, "Hello World!", new Vector2(1, 1), Color.White, 0, new Vector2(0, 0), new Vector2(1), 2f, strScissor);
    sb.End();

    cl.Begin();
    cl.SetFramebuffer(gd.SwapchainFramebuffer);
    cl.ClearColorTarget(0, RgbaFloat.CornflowerBlue);
    cl.ClearDepthStencil(0f);
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