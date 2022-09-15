using Veldrid;

namespace Peridot;

/// <summary>
/// Peridot command list extensions.
/// </summary>
public static class PeridotCommandListExtensions
{
	/// <summary>
	/// Draw this branch into a <see cref="CommandList"/>.
	/// Call this after calling <see cref="SpriteBatch{TTexture}.End"/>
	/// </summary>
	/// <param name="spriteBatch"></param>
	/// <param name="commandList"></param>
	public static void DrawBatch(this CommandList commandList, ISpriteBatch spriteBatch)
	{
		if (spriteBatch is not VSpriteBatch sb)
			throw new NotSupportedException();

		sb.DrawBatch(commandList);
	}
}
