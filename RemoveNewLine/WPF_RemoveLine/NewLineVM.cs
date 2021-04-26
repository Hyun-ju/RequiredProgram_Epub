using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WPF_RemoveLine
{
	public class NewLineVM : ViewModelBase
	{
		#region feild
		private string _textBox1;
		public string TextBox1
		{
			get { return _textBox1; }
			set { Set(ref _textBox1, value); }
		}

		private string _textBox2;
		public string TextBox2
		{
			get { return _textBox2; }
			set { Set(ref _textBox2, value); }
		}
		#endregion
		#region command
		public RelayCommand CmdConvert { get; set; }
		public RelayCommand CmdCopy { get; set; }
		public RelayCommand CmdClear { get; set; }
		#endregion

		public NewLineVM()
		{
			CmdConvert = new RelayCommand(Convert);
			CmdCopy = new RelayCommand(Copy);
			CmdClear = new RelayCommand(Clear);

		}

		public void Convert()
		{
			var content = new List<string>();
			var splitString = new string[] { "\r\n", "<p>", "</p>" };
			content = TextBox1.Split(splitString, StringSplitOptions.RemoveEmptyEntries).ToList();

			for (int i = 0; i < content.Count; i++) content[i] = content[i].Trim();
			for (int i = content.Count - 1; i > 0; i--)
			{
				if (string.IsNullOrEmpty(content[i])) content.RemoveAt(i);
				else if (content[i].StartsWith(" ")) content.RemoveAt(i);
			}

			//개행 조건 설정
			for (int i = content.Count - 1; i > 0; i--)
			{
				var stringA = content[i];
				var stringB = content[i - 1];

				//페이지번호, "/'
				if (int.TryParse(stringA, out var pageNumA)
					|| stringA.StartsWith("\"")
					|| stringA.StartsWith("\'")
					|| stringA.StartsWith("“")
					|| stringA.StartsWith("‘")
					|| int.TryParse(stringB, out var pageNumB)
					|| stringB.EndsWith("\"")
					|| stringB.EndsWith("\'")
					|| stringB.EndsWith("”")
					|| stringB.EndsWith("’")) continue;

				//제N장, 제N부
				if ((stringA.EndsWith("장") || stringA.EndsWith("부"))
					|| (stringB.EndsWith("장") || stringB.EndsWith("부")))
				{
					if ((stringB.StartsWith("제 ") || stringB.StartsWith("제"))
						|| (stringA.StartsWith("제 ") || stringA.StartsWith("제"))) continue;
					else if (int.TryParse(stringA[stringA.Length - 2].ToString(), out var intA)
						|| int.TryParse(stringB[stringB.Length - 2].ToString(), out var intB)) continue;
				}

				//각주, 미주
				if (stringA.StartsWith("각주")
					|| stringA.StartsWith("미주")
					|| stringA.StartsWith("주*")
					|| stringA.StartsWith("*")
					|| stringA.StartsWith("(미주")
					|| stringA.StartsWith("(각주")
					|| stringA.StartsWith("(주*")
					|| stringA.StartsWith("(*")) continue;

				if (stringB.EndsWith(".")) continue;
				if (stringB.EndsWith("!")) continue;

				content[i - 1] += stringA;
				content.RemoveAt(i);
			}

			//한자 및 영어 체크
			for (int row = 0; row < content.Count(); row++)
			{
				var nowRow = content[row];
				var strContent = string.Empty;
				int startIndex = -1;
				for (int i = 0; i < nowRow.Length; i++)
				{
					if (i == 0 || nowRow[i] == ' ')
					{
						strContent += nowRow[i];
						continue;
					}

					if (IsHan(nowRow[i]))
					{
						if (IsKor(nowRow[i - 1]))
						{
							if (i + 1 < nowRow.Length && !IsHan(nowRow[i + 1]) && nowRow[i + 1] != ' ')
							{
								strContent += ("(" + nowRow[i] + ")");
								continue;
							}
							else if (i + 1 < nowRow.Length && i + 2 < nowRow.Length &&
								nowRow[i + 1] == ' ' && !IsHan(nowRow[i + 2]))
							{
								strContent += ("(" + nowRow[i] + ")");
								continue;
							}
							else
								strContent += ("(" + nowRow[i]);
							startIndex = i;
							continue;
						}
						else if (i + 1 < nowRow.Length && !IsHan(nowRow[i + 1]) && nowRow[i + 1] != ' ')
						{
							if (startIndex == -1) strContent += nowRow[i];
							else strContent += (nowRow[i] + ")");
						}
						else if (i + 2 < nowRow.Length && !IsHan(nowRow[i + 2]) && nowRow[i + 1] == ' ')
						{
							if (startIndex == -1) strContent += nowRow[i];
							else strContent += (nowRow[i] + ")");
						}
						else
						{
							strContent += nowRow[i];
						}
					}
					else strContent += nowRow[i];
				}
				content[row] = strContent;
			}

            // 유니코드 변환
            for (int i = 0; i < content.Count(); i++)
            {
				string checkStr = content[i];
				if (!checkStr.Contains("&lt;") && !checkStr.Contains("&gt;") && !checkStr.Contains("&amp;")) continue;

				var insertStr = checkStr.ToArray().ToList();
				for (int j = insertStr.Count - 1; j >= 0; j--)
				{
					if (insertStr[j] == '&' && insertStr[j + 2] == 't' &&  insertStr[j + 3] == ';')
                    {
						if (insertStr[j + 1] == 'l') insertStr[j] = '〈';
						else if (insertStr[j + 1] == 'g') insertStr[j] = '〉';

						insertStr.RemoveAt(j + 3);
						insertStr.RemoveAt(j + 2);
						insertStr.RemoveAt(j + 1);
					}
                    else if (insertStr[j] == '&' && insertStr[j + 1] == 'a' && insertStr[j + 2] == 'm' && insertStr[j + 3] == 'p' && insertStr[j + 4] == ';')
					{
						insertStr[j] = '＆';
						insertStr.RemoveAt(j + 4);
						insertStr.RemoveAt(j + 3);
						insertStr.RemoveAt(j + 2);
						insertStr.RemoveAt(j + 1);
					}
				}
				content[i] = new string(insertStr.ToArray());
            }

			//textBox2에 view
			TextBox2 = string.Empty;
			foreach (var item in content)
			{
				TextBox2 += ("<p>" + item + "</p>" + "\n");
			}
		}
		public void Copy()
		{
			System.Windows.Clipboard.SetText(TextBox2);

		}
		public void Clear()
		{
			TextBox1 = string.Empty;
			TextBox2 = string.Empty;
		}

		public bool IsKor(char checkChar)
		{
			return (checkChar >= '\uAC00' && checkChar <= '\uD7A3');
		}
		public bool IsHan(char checkChar)
		{
			bool ch1 = (checkChar >= '\u2E80' && checkChar <= '\u2EFF');
			bool ch2 = (checkChar >= '\u3400' && checkChar <= '\u4DB5');
			bool ch3 = (checkChar >= '\u4E00' && checkChar <= '\u9FFF');
			return ch1 || ch2 || ch3;
		}
	}
}