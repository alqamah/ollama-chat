   M S B A M L   `   `   `  ����C  ?MyWpfApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:OllamaChatAppOllamaChatApp  5   	       OllamaChatApp.MainWindow   5   	   W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:OllamaChatApp     5   	     � Title$  Ollama Chat��5   	   $	��450��6   $	��600��6*   .��5      � $��10q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��2* �6   �� 5      $
��Auto �6    � ColumnDefinitions 5   
   �� 5       $��* �6   .+�5$   
   �� $��0��6   $��Enter your prompt:��6   �� 5%   
   -   5         ��Name$ PromptTextBox��5%      $��1��5&       �� AcceptsReturn$
 True��5'       �� TextWrapping$
 Wrap=�5(      " �� VerticalScrollBarVisibility$
 Auto=�5)      $��0,0,0,10q�5*      �� 5-   
   -   5         ��Name$ 
SendButton��5-      $��2��5.      $��Send to Ollama��5/      $
��Left=�51      $
��10,5q�52      $��0,0,0,10q�53      �� 56   
   $��3��6   $��Ollama Response:��6   �� 57   
   $��4��6   $
��Auto=�6$   .��58      �� -   5         ��Name$ ResponseTextBlock��58      $
��Wrap=�59      
�������5:      5;      �� 5>      $��5��6   $��0,5,0,0q�6"   .@�5?      �� .��5@      �� -   5        $ StatusTextBlock��5@      $��Ready��65   5A      5B      5D      5E      