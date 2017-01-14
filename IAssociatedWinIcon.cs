using System.Drawing;

namespace AssociatedWinIcon
{
	public interface IAssociatedWinIcon
	{
		Icon GetIcon(string extension);
	}
}