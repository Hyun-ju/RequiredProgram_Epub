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

		private bool _isCheckAll = true;
		public bool IsCheckAll
		{
			get { return _isCheckAll; }
			set { 
				Set(ref _isCheckAll, value);
				IsCheckQue = value;
				IsCheckQuo = value;
				IsCheckBra = value;
			}
		}
		private bool _isCheckQue = true;
		public bool IsCheckQue
		{
			get { return _isCheckQue; }
			set { Set(ref _isCheckQue, value); }
		}
		private bool _isCheckQuo = true;
		public bool IsCheckQuo
		{
			get { return _isCheckQuo; }
			set { Set(ref _isCheckQuo, value); }
		}
		private bool _isCheckBra = true;
		public bool IsCheckBra
		{
			get { return _isCheckBra; }
			set { Set(ref _isCheckBra, value); }
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
				var stringCurrent = content[i];
				var stringPre = content[i - 1];

				//페이지번호
				if (int.TryParse(stringCurrent, out var pageNumA)
					|| int.TryParse(stringPre, out var pageNumB)) continue;

				//따옴표
				if (IsCheckQuo
					&& (stringCurrent.StartsWith("\"")
					|| stringCurrent.StartsWith("\'")
					|| stringCurrent.StartsWith("“")
					|| stringCurrent.StartsWith("‘")
					|| stringPre.EndsWith("\"")
					|| stringPre.EndsWith("\'")
					|| stringPre.EndsWith("”")
					|| stringPre.EndsWith("’"))) continue;

				//[ 및 ]
				if (IsCheckBra
					&& (stringCurrent.StartsWith("[")
					|| stringPre.EndsWith("]"))) continue;

				//제N장, 제N부
				if (stringPre.Length >= 4 && stringPre.StartsWith("제 ") && char.IsNumber(stringPre[2]))
				{
					int ind = IsNum(stringPre, 2);
					if (ind < stringPre.Length && IsHeader(stringPre, ind)) continue;
					else if (ind + 1 < stringPre.Length && IsHeader(stringPre, ind + 1)) continue;
				}
				else if (stringCurrent.Length >= 4 && stringCurrent.StartsWith("제 ") && char.IsNumber(stringCurrent[2]))
				{
					int ind = IsNum(stringCurrent, 2);
					if (ind < stringCurrent.Length && IsHeader(stringCurrent, ind)) continue;
					else if (ind + 1 < stringCurrent.Length && IsHeader(stringCurrent, ind + 1)) continue;
				}
				else if (stringPre.Length >= 3 && stringPre.StartsWith("제") && char.IsNumber(stringPre[1]))
				{
					int ind = IsNum(stringPre, 1);
					if (ind < stringPre.Length && IsHeader(stringPre, ind)) continue;
					else if (ind + 1 < stringPre.Length && IsHeader(stringPre, ind + 1)) continue;
				}
				else if (stringCurrent.Length >= 3 && stringCurrent.StartsWith("제") && char.IsNumber(stringCurrent[1]))
				{
					int ind = IsNum(stringCurrent, 1);
					if (ind < stringCurrent.Length && IsHeader(stringCurrent, ind)) continue;
					else if (ind + 1 < stringCurrent.Length && IsHeader(stringCurrent, ind + 1)) continue;
				}


				//각주, 미주
				if (stringCurrent.StartsWith("각주")
					|| stringCurrent.StartsWith("미주")
					|| stringCurrent.StartsWith("주*")
					|| stringCurrent.StartsWith("*")
					|| stringCurrent.StartsWith("(미주")
					|| stringCurrent.StartsWith("(각주")
					|| stringCurrent.StartsWith("(주*")
					|| stringCurrent.StartsWith("(*")) continue;

				if (stringPre.EndsWith(".")) continue;
				if (stringPre.EndsWith("!")) continue;
				if (IsCheckQue && stringPre.EndsWith("?")) continue;

				content[i - 1] += stringCurrent;
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

		public int IsNum(string str, int indexN)
		{
			if (str.Length > indexN && char.IsNumber(str[indexN])) 
				return IsNum(str, indexN + 1);
			else return indexN;
		}
		//부 및 장 판단 함수
		public bool IsHeader(string str, int indexN)
		{
			return str[indexN] == '부' || str[indexN] == '장';
		}
		//한글 판단 함수
		public bool IsKor(char checkChar)
		{
			return (checkChar >= '\uAC00' && checkChar <= '\uD7A3');
		}
		//한문 판단 함수
		public bool IsHan(char checkChar)
		{
			bool ch1 = (checkChar >= '\u2E80' && checkChar <= '\u2EFF');
			bool ch2 = (checkChar >= '\u3400' && checkChar <= '\u4DB5');
			bool ch3 = (checkChar >= '\u4E00' && checkChar <= '\u9FFF');
			return ch1 || ch2 || ch3;
		}
	}
}