   M S B A M L   `   `   `  ���� G  COrienteering, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:Orienteering.ViewsOrienteering.Views  P LElysium, Version=2.0.976.2, Culture=neutral, PublicKeyToken=afa220db249e5b15D/clr-namespace:Elysium.Controls;assembly=ElysiumElysium.Controls =#clr-namespace:Orienteering.ControlsOrienteering.Controls      Orienteering.MainWindow   'my clr-namespace:Orienteering.Views  R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Ametro/clr-namespace:Elysium.Controls;assembly=Elysium     7params*http://schemas.codeplex.com/elysium/params  /local#clr-namespace:Orienteering.Controls  -     � Title$   ��$	��900��$
��1440�� � WindowStartupLocation$ CenterScreen=�  Elysium.Parameters.Design   Theme$ LightX�   Elysium.Parameters.Manager   Theme$ LightX� � WindowState$ 	Maximized=�  Elysium.Controls.Window   TitleBar �  � ColumnDefinitions �� $��* ��� $
��Auto �.+��� -   $��1��$��?��&   MainWindowCaptionButtonStyleKey#���  .���  � RowDefinitions �� $	��55* ��� $	��30* �.+�'   !Orienteering.Controls.SliderPanel  -   	  Name$	 sliderPanel��$��2�����	#00000000.+�]� .+��  �� $
��80px ��� $��1* � �� $	��50* ��� $	��50* �.+��� $(��"Resources/Fonts/seoge_ui_light.ttf)�$��20,0,0,0q�$��36(�$��Orienteering��$��50,0,0,0q�� -   $��infoGrid��$y�Hidden=�$��1�� �� ��  �� �� $	��250 �.+��� $��36��$��36��$��Right=�$��0,0,10,0q�$;��5/Orienteering;component/Resources/Images/question.png���� $��1��$(��"Resources/Fonts/seoge_ui_light.ttf)�$��12(�$2��,w bazie danych znajdują się otwarte zawody��$��Center=�$��Right=�$��0,0,10,0q��� -   $��1��$��1��$��30��$��80��$��Right=�$��0,0,10,0q�$��Pokaż��� $��2��$��1��$��2��$	��350��$	��350�� �� $��1* ��� $��1* � �� $��1* ��� $��1* �.+�]� -   $��10q�
 � Cursor$

 Hand�����	#FF389DE2.+��� $��100px��$	��Top=�$��Resources/Images/qr.jpg��$
��Fill=��� $
��50px��$��Generator kodów QR��$��Center=�$��Center=�$(��"Resources/Fonts/seoge_ui_light.ttf)�
�������$��14(�]� -   $��1��$��10q�$

 Hand�����	#FF3CAE1A.+��� $��100px��$	��Top=�$ ��Resources/Images/route.jpg��$
��Fill=��� $
��50px��$��Wyznaczanie trasy��$��Center=�$��Center=�$(��"Resources/Fonts/seoge_ui_light.ttf)�
�������$��14(�]� -   $��1��$��10q�$

 Hand�����	#FF781AAE.+��� $��100px��$	��Top=�$��Resources/Images/user.jpg��$
��Fill=��� $
��50px��$ ��Zarządzenie
uczestnikami��$��Center=�$��Center=�$(��"Resources/Fonts/seoge_ui_light.ttf)�
�������$��14(�]� -	   $��1��$��1��$��10q�$

 Hand�����	#FFAE1A50.+��� $��100px��$	��Top=�$"��Resources/Images/contest.png��$
��Fill=��� $
��50px��$��Kreator zawodów��$��Center=�$��Center=�$(��"Resources/Fonts/seoge_ui_light.ttf)�
�������$��14(��� $��1��$	��600��$	��600��$
��Left=�$��Bottom=�$!��Resources/Images/Arrows.png��$
��Fill=�]� .+��  �� $
��80px ��� $��1* �.+��� -
   $��32��$��32��$
��Left=�$	��Top=�$=��7/Orienteering;component/Resources/Images/back_arrow.png��$��15,5,0,0q�$

 Hand���� $��2��$(��"Resources/Fonts/seoge_ui_light.ttf)�$��20,0,0,0q�$��36(�$��Generator kodów QR��$��50,0,0,0q�#   Orienteering.Views.QRCodeView  $��1��$��	50,0,50,0q�]� .+��  �� $
��80px ��� $��1* �.+��� -   $��32��$��32��$
��Left=�$	��Top=�$=��7/Orienteering;component/Resources/Images/back_arrow.png��$��15,5,0,0q�$

 Hand���� $��2��$(��"Resources/Fonts/seoge_ui_light.ttf)�$��20,0,0,0q�$��36(�$��Wyznaczanie trasy��$��50,0,0,0q�#   Orienteering.Views.RoutesView  $��1��$��	50,0,50,0q�]� .+��  �� $
��80px ��� $��1* �.+��� -   $��32��$��32��$
��Left=�$	��Top=�$=��7/Orienteering;component/Resources/Images/back_arrow.png��$��15,5,0,0q�$

 Hand���� $��2��$(��"Resources/Fonts/seoge_ui_light.ttf)�$��20,0,0,0q�$��36(�$��Zarządzanie uczestnikami��$��50,0,0,0q�"   Orienteering.Views.UsersView  $��1��$��	50,0,50,0q�]� .+��  �� $
��80px ��� $��1* �.+��� -   $��32��$��32��$
��Left=�$	��Top=�$=��7/Orienteering;component/Resources/Images/back_arrow.png��$��15,5,0,0q�$

 Hand���� $��2��$(��"Resources/Fonts/seoge_ui_light.ttf)�$��20,0,0,0q�$��36(�$��Kreator zawodów��$��50,0,0,0q�$   Orienteering.Views.ContestView  -     Name$ contestView��$	   Orienteering.Views.ContestView 	  Loaded$ contestView_Loaded��$��1��$��	50,0,50,0q�