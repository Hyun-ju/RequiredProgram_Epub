   M S B A M L   `   `   `  ���� E  ARemoveLine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:WPF_RemoveLineWPF_RemoveLine  /clr-namespace:WPF_RemoveLineWPF_RemoveLine      WPF_RemoveLine.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     .localclr-namespace:WPF_RemoveLine     +vmclr-namespace:WPF_RemoveLine     -   $��VM��  � Icon$  logo.ico�� � Title$ 개행 지우기��$	��525��$	��700�� � DataContext    WPF_RemoveLine.NewLineVM  .���  � ColumnDefinitions �� $��1* ��� $��70 ��� $��1* � � RowDefinitions �� $��10 ��� $��35 ��� $��20 ��� $��1* ��� $��50 �.+��� $��0��$��3��$��0�� �� Orientation$ 
Horizontal=�.+��� $��ⓒHyunjuKim��$��8(�$��5 0 0 0q��� $ 
Horizontal=�$��1��$��3��$��Center=�$��Center=�.+��� $��Center=�$��개행 조건: ��$��14(�$��0 0 10 0q��� $��Center=�$��모두 체크/풀기��$��14(�$��5 0 8 0q� d� 	IsChecked �� *
IsCheckAll+�� $��Center=����� *
CheckBoxes+���� .R�$�  $� Orientation$ 
Horizontal=����� .R��� $��Center=�$��14(�$��돋움)�$��8 0 8 0q����� *Text+ �� *	IsChecked+�� $��0��$��2��$ 
Horizontal=�$��Center=�$��Center=�.+��� $��변환 전 내용 입력��$��10(��� $��2��$��2��$ 
Horizontal=�$��Center=�$��Center=�.+��� $��
변환 후��$��10(��� $��0��$��3��$��5 0 0 0q�.+��� " �� VerticalScrollBarVisibility$
 Auto=�	 �� TextWrapping$
	 Wrap=�
 �� AcceptsReturn
 . $	��370������ *
TextBox1+�� $��2��$��3��$��0 0 5 0q�.+��� $	��370��$
 Auto=�$
	 Wrap=�
 . ���� *
TextBox2+�� $��1��$��3��$��Center=�.+��� $��변환하기��$��60��$��50������ *
CmdConvert+�� $��4��$��3��$ 
Horizontal=�$��Center=�$��Right=�.+��� $��복사하기��$��90��$��30��$��0 0 10 0q����� *	CmdCopy+�� $��내용 지우기��$��90��$��30��$��	10 0 10 0q����� *
CmdClear+