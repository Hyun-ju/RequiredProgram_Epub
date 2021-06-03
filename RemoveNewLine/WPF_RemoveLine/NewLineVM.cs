using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace WPF_RemoveLine
{
	public class CheckBoxList:ObservableObject
	{
		public string Text { get; set; }
		private bool _isCheck = true;
		public bool IsChecked
		{
			get { return _isCheck; }
			set { Set(ref _isCheck, value); }
		}

		public CheckBoxList(string text)
		{
			Text = text;
		}
	}

	public class NewLineVM : ViewModelBase
	{
		#region feild
		readonly Regex KorRegex = new Regex("[가-힣]|[ㄱ-ㅎ]");
		readonly Regex HanRegex = new Regex("[\u2E80-\u2EFF]|[\u3400-\u4DB5]|[\u4E00-\u9FFF]");
		readonly Regex EngRegex = new Regex("[a-zA-Z]");
		readonly Regex EndMarkRegex = new Regex("[.,!?]");
		readonly Regex StartQuoRegex = new Regex("[\"\'‘“]");
		readonly Regex EndQuoRegex = new Regex("[\"\'’”]");
		readonly Regex NumInCirRegex = new Regex("[①-⑮]");

		readonly List<char> OpenBraList = new List<char>() { '｢', '[', '〔', '〈', '《', '「', '『', '【' };
		readonly List<char> EndBraList = new List<char>() { '｣', ']', '〕', '〉', '》', '」', '』', '】' };
		//readonly Regex OpenBraRegex = new Regex("[〔〈《「『【]");
		//readonly Regex EndBraRegex = new Regex("[〕〉》」』】]");

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


		private bool _isTable;
		public bool IsTable
		{
			get { return _isTable; }
			set { Set(ref _isTable, value); }
		}

		private bool _isCheckAll = true;
		public bool IsCheckAll
		{
			get { return _isCheckAll; }
			set
			{
				Set(ref _isCheckAll, value);
				foreach (var item in CheckBoxes)
					item.IsChecked = value;
			}
		}

		private List<CheckBoxList> _checkBoxes = new List<CheckBoxList>();
		public List<CheckBoxList> CheckBoxes
		{
			get
			{ return _checkBoxes; }
			set { Set(ref _checkBoxes, value); }
		}
		#endregion
		#region command
		public RelayCommand CmdConvert { get; set; }
		public RelayCommand CmdCopy { get; set; }
		public RelayCommand CmdClear { get; set; }
		#endregion

		public NewLineVM()
		{
			CheckBoxes.Add(new CheckBoxList("?"));
			CheckBoxes.Add(new CheckBoxList("“” 및 ‘’"));
			CheckBoxes.Add(new CheckBoxList("[ ]"));
			CheckBoxes.Add(new CheckBoxList("N)"));
			CheckBoxes.Add(new CheckBoxList("(N)"));
			CheckBoxes.Add(new CheckBoxList("ⓝ"));
			CheckBoxes.Add(new CheckBoxList("•"));

			CmdConvert = new RelayCommand(Convert);
			CmdCopy = new RelayCommand(Copy);
			CmdClear = new RelayCommand(Clear);
		}

		#region 판단 함수 모음
		public int IsNum(string str, int indexN)
		{
			if (str.Length > indexN && char.IsNumber(str[indexN]))
				return IsNum(str, indexN + 1);
			if (str.Length <= indexN) return indexN - 1;
			else return indexN;
		}
		//부 및 장 판단 함수
		public bool IsHeader(string str, int indexN)
		{
			return str[indexN].Equals('부') || str[indexN].Equals('장');
		}
		#endregion

		public void Convert()
		{
			if (!IsTable) LineConvert();
			else CreateTable();
		}

		public void LineConvert()
		{
			var content = new List<string>();
			var splitString = new string[] { "\r\n", "<p>", "</p>" };
			content = TextBox1.Split(splitString, StringSplitOptions.RemoveEmptyEntries).ToList();

			for (int i = 0; i < content.Count; i++) content[i] = content[i].Trim();
			for (int i = content.Count - 1; i >= 0; i--)
			{
				if (string.IsNullOrEmpty(content[i]) || string.IsNullOrWhiteSpace(content[i]) || content[i].Equals("​")) content.RemoveAt(i);
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
				if (CheckBoxes[1].IsChecked
					&& (StartQuoRegex.IsMatch(stringCurrent[0].ToString()) 
						|| EndQuoRegex.IsMatch(stringPre[stringPre.Length-1].ToString()))) continue;

				//[ 및 ]
				if (CheckBoxes[2].IsChecked
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


				//1)
				if (CheckBoxes[3].IsChecked && ((char.IsNumber(stringCurrent[0]) && stringCurrent[IsNum(stringCurrent, 0)].Equals(')'))
					|| (char.IsNumber(stringPre[0]) && stringPre[IsNum(stringPre, 0)].Equals(')')))) continue;
				//(1)
				if (CheckBoxes[4].IsChecked &&
					((stringCurrent[0].Equals('(')
				&& char.IsNumber(stringCurrent[1]) && stringCurrent[IsNum(stringCurrent, 1)].Equals(')'))
				|| (stringPre[0].Equals('(')
				&& char.IsNumber(stringPre[1]) && stringPre[IsNum(stringPre, 1)].Equals(')')))) continue;
				//①
				if (CheckBoxes[5].IsChecked &&
					NumInCirRegex.IsMatch(stringCurrent[0].ToString()) || NumInCirRegex.IsMatch(stringPre[0].ToString())) continue;

				//•
				if (CheckBoxes[6].IsChecked &&
					stringCurrent.StartsWith("•") || stringPre.StartsWith("•")) continue;

				//각주, 미주
				if (stringCurrent.StartsWith("각주")
					|| stringCurrent.StartsWith("미주")
					|| stringCurrent.StartsWith("주*")
					|| stringCurrent.StartsWith("*")
					|| stringCurrent.StartsWith("(미주")
					|| stringCurrent.StartsWith("(각주")
					|| stringCurrent.StartsWith("(주*")
					|| stringCurrent.StartsWith("(*")) continue;


				//<span> <h>태그
				if (stringCurrent.StartsWith("<span") || stringPre.StartsWith("<span")
					|| stringCurrent.StartsWith("<h") || stringPre.StartsWith("<h")) continue;


				if (stringPre.EndsWith(".")) continue;
				if (stringPre.EndsWith("!")) continue;
				if (CheckBoxes[0].IsChecked && stringPre.EndsWith("?")) continue;

				content[i - 1] += stringCurrent;
				content.RemoveAt(i);
			}

			//한자 체크
			for (int row = 0; row < content.Count(); row++)
			{
				var nowRow = content[row];

				if (!HanRegex.IsMatch(nowRow)) continue;
				bool isOpen = false;
				for (int i = 1; i < nowRow.Length; i++)
				{
					if (!HanRegex.IsMatch(nowRow[i].ToString())) continue;

					if (!isOpen && KorRegex.IsMatch(nowRow[i - 1].ToString()))
					{
						nowRow = nowRow.Insert(i, "(");
						isOpen = true;
					}
					if (isOpen && i + 2 < nowRow.Length
						&& !HanRegex.IsMatch(nowRow[i + 1].ToString()) && !HanRegex.IsMatch(nowRow[i + 2].ToString()))
					{
						nowRow = nowRow.Insert(i + 1, ")");
						isOpen = false;
					}
				}

				content[row] = nowRow;
			}

			//유니코드 변환
			var convertUnicode = new List<List<string>>()
			{
				new List<string>(){ "&lt;", "〈" },
				new List<string>(){ "&gt;", "〉" },
				new List<string>(){ "&amp;", "＆" },
				new List<string>(){ "“ ", " “" },
				new List<string>(){ " ”", "”" },
				new List<string>(){ "‘ ", " ‘" },
				new List<string>(){ " ’", "’" },
			};
			foreach (var item in OpenBraList)
			{
				convertUnicode.Add(new List<string>() { item.ToString(), (" " + item) });
				convertUnicode.Add(new List<string>() { (item + " "), item.ToString() });
			}
			foreach (var item in EndBraList)
				convertUnicode.Add(new List<string>() { (" " + item), item.ToString() });

			for (int i = 0; i < content.Count(); i++)
			{
				string checkStr = content[i];

				foreach (var item in convertUnicode)
				{
					if (checkStr.Contains(item[0]))
						checkStr = checkStr.Replace(item[0], item[1]);
				}

				checkStr = CheckEndMarkAndApos(checkStr, 0);
				if (checkStr.Contains("  ")) checkStr = MuntiBlankSpace(checkStr);

				content[i] = checkStr.Trim();
			}


			//textBox2에 view
			TextBox2 = string.Empty;
			foreach (var item in content)
			{
				if (item.StartsWith("<span") || item.StartsWith("<h")) TextBox2 += (item + "\n\n");
				else TextBox2 += ("<p>" + item + "</p>" + "\n\n");
			}
		}

		public void CreateTable()
		{
			var stringList = new List<string>();
			stringList = TextBox1.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

			for (int i = 0; i < stringList.Count; i++)
			{
				stringList[i] = stringList[i].Trim().Replace(" ", "</td><td>");	
				stringList[i] = stringList[i].Insert(0, "<tr>\n<td>");
				stringList[i] += "</td>\n</tr>";
			}


			TextBox2 = "<table>\n<caption>[표]</caption>\n<thead><tr><th>필요하면 쓰세용..</th></tr></thead>\n<tbody>";
			foreach (var item in stringList)
			{
				TextBox2 += (item + "\n");
			}
			TextBox2 += "</tbody>\n</table>";
		}

		public string MuntiBlankSpace(string checkString)
		{
			checkString = checkString.Replace("  ", " ");
			if (checkString.Contains("  ")) MuntiBlankSpace(checkString);
			return checkString;
		}
		public string CheckEndMarkAndApos(string checkString, int checkIndex)
		{
			if (checkIndex >= checkString.Length - 1) return new string(checkString.ToArray());
			if (EndMarkRegex.IsMatch(checkString[checkIndex].ToString()) && !checkString[checkIndex + 1].Equals(' '))
				checkString = checkString.Insert(checkIndex + 1, " ");
			if ((checkString[checkIndex].Equals('‘') || checkString[checkIndex].Equals('’'))
				 && checkIndex > 0 && checkIndex < checkString.Length - 1 && EngRegex.IsMatch(checkString[checkIndex - 1].ToString()))
			{
				if (EngRegex.IsMatch(checkString[checkIndex + 1].ToString())
					|| (checkIndex < checkString.Length - 2 && char.IsWhiteSpace(checkString[checkIndex + 1]) && EngRegex.IsMatch(checkString[checkIndex + 1].ToString())))
				{
					checkString = checkString.Remove(checkIndex, 1);
					checkString = checkString.Insert(checkIndex, "\'");
				}
			}

			CheckEndMarkAndApos(checkString, ++checkIndex);
			return new string(checkString.ToArray());
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
	}
}