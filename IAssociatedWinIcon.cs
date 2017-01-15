using System.Collections.Generic;
using System.Drawing;

namespace AssociatedWinIcon
{
	public interface IAssociatedWinIcon
	{
		Icon GetIcon(string extension);

		IEnumerable<string> GetRegExtensions();
	}
}