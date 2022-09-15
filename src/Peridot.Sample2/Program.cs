using System.Drawing;
using Peridot.Text;
using System.Numerics;
using Veldrid.StartupUtilities;
using Veldrid;
using Peridot;
using System.Globalization;

internal class Program
{
	public class Unit
	{
		public Vector2 Position;
		public Vector2 Speed;
	}
	
	private static Dictionary<Color, List<Unit>> Groups = new();
	private static Dictionary<(Color, Color), float> Rules = new();
	private static Random rnd = new();
	private static Size Bounds;
	private static readonly float Gold = (MathF.Sqrt(5) + 1f) / 2f;
	
	private static void Rule(float deltaTime, List<Unit> units1, List<Unit> units2, float g)
	{
		units1.AsParallel().ForAll(a =>
		{
			var force = new Vector2();
			
			for (var j = 0; j < units2.Count; j++)
			{
				var b = units2[j];
				
				var diff = a.Position - b.Position;
				var distance = Vector2.Distance(a.Position, b.Position);
				if (distance > 0 && distance < 80)
				{
					var F = g * (1f / distance);
					force += diff * F;
				}
			}
			
			a.Speed += force * 0.5f;
			a.Position += a.Speed;
			
			var m = 0.5f;
			
			if (((a.Position.X <= 0 && a.Speed.X < 0) || (a.Position.X >= Bounds.Width && a.Speed.X > 0)))
				a.Speed.X *= -m;
			else
				a.Speed.X *= m;
				
			if (((a.Position.Y <= 0 && a.Speed.Y < 0) || (a.Position.Y >= Bounds.Height && a.Speed.Y > 0)))
				a.Speed.Y *= -m;
			else
				a.Speed.Y *= m;
		});
	}
	
	private static void AddUnits(int count, Color color)
	{
		if (!Groups.TryGetValue(color, out var units))
		{
			Groups[color] = units = new();
		}
		
		for(var i = 0; i < count; i++)
		{
			var x = rnd.Next(0, Bounds.Width);
			var y = rnd.Next(0, Bounds.Height);
			
			units.Add(new() 
			{
				Position = new(x, y)
			});
		}
	}
	
	private static void Load()
	{
		Groups.Clear();
		AddUnits(500, Color.Yellow);
		AddUnits(500, Color.Red);
		AddUnits(500, Color.Green);
		AddUnits(500, Color.White);
		AddUnits(500, Color.Purple);
	}
	
	private static void Gen() 
	{
		var colors = new Color[] { Color.Yellow, Color.Red, Color.Green, Color.White, Color.Purple };
		Rules.Clear();
		Console.WriteLine();
		Console.WriteLine();
		Console.WriteLine();
		Console.WriteLine("New Rules!");
		Console.WriteLine();
		foreach (var color1 in colors)
			foreach(var color2 in colors)
			{
				var rule = rnd.NextSingle() - 0.5f;
				Rules.Add((color1, color2), rule);
				Console.WriteLine($"Rules.Add((Color.{color1.Name}, Color.{color2.Name}), {rule.ToString(CultureInfo.InvariantCulture)}f);");
			}
	}
	
	private static void VirusRule() 
	{
		Rules.Clear();
		Rules.Add((Color.Yellow, Color.Yellow), 0.16813803f);
		Rules.Add((Color.Yellow, Color.Red), -0.13937545f);
		Rules.Add((Color.Yellow, Color.Green), 0.15008587f);
		Rules.Add((Color.Yellow, Color.White), -0.3598255f);
		Rules.Add((Color.Red, Color.Yellow), 0.12564862f);
		Rules.Add((Color.Red, Color.Red), 0.096319795f);
		Rules.Add((Color.Red, Color.Green), -0.12434393f);
		Rules.Add((Color.Red, Color.White), -0.23231715f);
		Rules.Add((Color.Green, Color.Yellow), 0.46440452f);
		Rules.Add((Color.Green, Color.Red), 0.38713688f);
		Rules.Add((Color.Green, Color.Green), -0.36697257f);
		Rules.Add((Color.Green, Color.White), -0.033506453f);
		Rules.Add((Color.White, Color.Yellow), -0.35188568f);
		Rules.Add((Color.White, Color.Red), 0.1912772f);
		Rules.Add((Color.White, Color.Green), 0.3761289f);
		Rules.Add((Color.White, Color.White), 0.059764445f);
	}
		
	private static void Draw(float deltaTime, ISpriteBatch spriteBatch)
	{
		foreach(var rule in Rules)
		{
			if (!Groups.TryGetValue(rule.Key.Item1, out var units1) ||
				!Groups.TryGetValue(rule.Key.Item2, out var units2))
				continue;
			
			Rule(deltaTime, units1, units2, rule.Value);
		}
		
		foreach(var pair in Groups)
		{
			foreach(var unit in pair.Value)
			{
				var rect = new RectangleF(unit.Position.X, unit.Position.Y, 4, 4);
				
				var colorF = new ColorF(pair.Key) / 2;
				var length = unit.Speed.Length();
				colorF += colorF * length/ (length + 50);
				colorF.A = 0.5f;
				
				spriteBatch.DrawRect(rect, colorF, 1);
			}
		}
	}

	private static void Main(string[] args)
	{
		var title = "Peridot.Sample2";
		var wci = new WindowCreateInfo(100, 100, 640, 480, WindowState.Normal, title);

		var window = VeldridStartup.CreateWindow(wci);
		var gd = VeldridStartup.CreateVulkanGraphicsDevice(
			new(true,
				Veldrid.PixelFormat.D32_Float_S8_UInt,
				false,
				ResourceBindingModel.Default,
				true,
				true),
			window);

		Bounds = new Size(window.Width, window.Height);
		window.Resized += () =>
		{
			Bounds = new Size(window.Width, window.Height);
			gd.MainSwapchain.Resize((uint)window.Width, (uint)window.Height);
		};
		
		window.KeyDown += (e) => 
		{
			if (e.Key == Key.G)	
				Gen();
			else if (e.Key == Key.V)
				VirusRule();
			else if (e.Key == Key.R)
				Load();
		};
		

		var peridot = new VPeridot(gd);

		var shaders = peridot.LoadDefaultShaders();
		var sbd = new SpriteBatchDescriptor(gd.MainSwapchain.Framebuffer.OutputDescription, shaders);
		var sb = peridot.CreateSpriteBatch(sbd);

		var tr = new TextRenderer(peridot, sb);
		var cl = gd.ResourceFactory.CreateCommandList();
		var fence = gd.ResourceFactory.CreateFence(false);
		
		var time = DateTime.Now;
		
		Load();
		var count = 0.0;
		while (window.Exists)
		{
			var now = DateTime.Now;
			var delta = now - time;
			var deltaTime = delta.TotalSeconds;
			time = now;
			count += deltaTime;

			if (count > 1)
			{
				count -= 1;
				window.Title = title + " " + (1f / deltaTime);
			}

			window.PumpEvents();

			sb.Begin();
			sb.ViewMatrix = Matrix4x4.CreateOrthographicOffCenter(0, Bounds.Width, 0, Bounds.Height, 0.01f, -100f);

			Draw((float)deltaTime, sb);
			sb.End();

			cl.Begin();
			cl.SetFramebuffer(gd.SwapchainFramebuffer);
			cl.ClearColorTarget(0, RgbaFloat.Black);
			cl.ClearDepthStencil(0f);
			cl.DrawBatch(sb);
			cl.End();

			fence.Reset();
			gd.SubmitCommands(cl, fence);
			gd.WaitForFence(fence);
			gd.SwapBuffers();
		}
	}
}
