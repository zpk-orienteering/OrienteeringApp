   M S B A M L   `   `   `  ����G  COrienteering, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:Orienteering.ViewsOrienteering.Views  5      P LElysium, Version=2.0.976.2, Culture=neutral, PublicKeyToken=afa220db249e5b15D/clr-namespace:Elysium.Controls;assembly=ElysiumElysium.Controls 5   	   =#clr-namespace:Orienteering.ControlsOrienteering.Controls  5   	       Orienteering.MainWindow   5      'my clr-namespace:Orienteering.Views  R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   Ametro/clr-namespace:Elysium.Controls;assembly=Elysium     5   	   7params*http://schemas.codeplex.com/elysium/params  5   	   /local#clr-namespace:Orienteering.Controls  5   	   -   5          � Title$   ��5   	   $	��900��6   $
��1440��6    � WindowStartupLocation$ CenterScreen=�5   	     Elysium.Parameters.Design   Theme$ LightX�5	   	      Elysium.Parameters.Manager   Theme$ LightX�5
   	    � WindowState$ 	Maximized=�6&     Elysium.Controls.Window   TitleBar 5      � 5   
    � ColumnDefinitions 5      �� 5      $��* �6#   �� 5      $
��Auto �6#   .+�5      �� -   5        $��1��5      $��?��5      &   MainWindowCaptionButtonStyleKey#���  5      5      .��5      �  � RowDefinitions 5   
   �� 5      $	��55* �6   �� 5      $	��30* �6   .+�5   
   '   !Orienteering.Controls.SliderPanel  -   5        	  Name$	 sliderPanel��5   -   $��2��6   ���	#00000000.+�5      ]� .+�5      �  5       �� 5!      $
��80px �6(   �� 5"      $��1* �6(    5$      �� 5%      $	��50* �6+   �� 5&      $	��50* �6+   .+�5(      �� $(��"Resources/Fonts/seoge_ui_light.ttf)�6   $��20,0,0,0q�6M   $��36(�6`   $��Orienteering��6n   $��50,0,0,0q�6�   � 5)      -   5        $��infoGrid��5)      $y�Hidden=�6+   $��1��6?    5*      �� 5+      �� 5,       5.      �� 5/      �� 50      $	��250 �6/   .+�52      �� $��36��6    $��36��6+   $��Right=�67   $��0,0,10,0q�6S   $;��5/Orienteering;component/Resources/Images/question.png��53       �� 54      $��1��6$   $(��"Resources/Fonts/seoge_ui_light.ttf)�55   $   $��12(�56   $   $2��,w bazie danych znajdują się otwarte zawody��57   $   $��Center=�58   $   $��Right=�59   $   $��0,0,10,0q�5:   $   �� 5;      -   5        $��1��5;   !   $��1��6.   $��30��5<   !   $��80��6-   $��Right=�5=   !   $��0,0,10,0q�6=   $��Pokaż��5>   !   61   5?      � 5A      $��2��6   $��1��6(   $��2��68   $	��350��6I   $	��350��6V    5B      �� 5C      $��1* �6,   �� 5D      $��1* �6,    5F      �� 5G      $��1* �6/   �� 5H      $��1* �6/   .+�5J      ]� -   5        $��10q�5J   $   
 � Cursor$

 Hand��60   ���	#FF389DE2.+�5K      �� $��100px��6$   $	��Top=�63   $��Resources/Images/qr.jpg��6H   $
��Fill=�6i   �� 5L      $
��50px��6$   $��Generator kodów QR��62   $��Center=�6O   $��Center=�6q   $(��"Resources/Fonts/seoge_ui_light.ttf)�6�   
�������$��14(�6�   5M      ]� 5N      -   5        $��1��5N   $   $��10q�64   $

 Hand��6@   ���	#FF3CAE1A.+�5O      �� $��100px��6$   $	��Top=�63   $ ��Resources/Images/route.jpg��6H   $
��Fill=�6l   �� 5P      $
��50px��6$   $��Wyznaczanie trasy��62   $��Center=�6N   $��Center=�6p   $(��"Resources/Fonts/seoge_ui_light.ttf)�6�   
�������$��14(�6�   5Q      ]� 5R      -   5        $��1��5R   $   $��10q�61   $

 Hand��6=   ���	#FF781AAE.+�5S      �� $��100px��6$   $	��Top=�63   $��Resources/Images/user.jpg��6H   $
��Fill=�6k   �� 5T      $
��50px��6$   $ ��Zarządzenie
uczestnikami��62   $��Center=�6^   $��Center=�6�   $(��"Resources/Fonts/seoge_ui_light.ttf)�6�   
�������$��14(�6�   5U      ]� 5V      -	   5        $��1��5V   $   $��1��61   $��10q�6A   $

 Hand��6M   ���	#FFAE1A50.+�5W      �� $��100px��6$   $	��Top=�63   $"��Resources/Images/contest.png��6H   $
��Fill=�6n   �� 5X      $
��50px��6$   $��Kreator zawodów��62   $��Center=�6L   $��Center=�6n   $(��"Resources/Fonts/seoge_ui_light.ttf)�6�   
�������$��14(�6�   5Y      5Z      �� 5[      $��1��6   $	��600��6)   $	��600��69   $
��Left=�6H   $��Bottom=�6c   $!��Resources/Images/Arrows.png��6~   $
��Fill=�6�   5\      5]      ]� 5^      .+�5_      �  5`      �� 5a      $
��80px �6(   �� 5b      $��1* �6(   .+�5d      �� -
   5        $��32��5d      $��32��6(   $
��Left=�63   $	��Top=�6N   $=��7/Orienteering;component/Resources/Images/back_arrow.png��5e      $��15,5,0,0q�5f      $

 Hand��6.   6<   �� 5g      $��2��6   $(��"Resources/Fonts/seoge_ui_light.ttf)�60   $��20,0,0,0q�6`   $��36(�6s   $��Generator kodów QR��6�   $��50,0,0,0q�6�   #   Orienteering.Views.QRCodeView  5h      $��1��6$   $��	50,0,50,0q�61   5i      5j      ]� 5k      .+�5l      �  5m      �� 5n      $
��80px �6(   �� 5o      $��1* �6(   .+�5q      �� -   5        $��32��5q      $��32��6(   $
��Left=�63   $	��Top=�6N   $=��7/Orienteering;component/Resources/Images/back_arrow.png��5r      $��15,5,0,0q�5s      $

 Hand��6.   6<   �� 5t      $��2��6   $(��"Resources/Fonts/seoge_ui_light.ttf)�60   $��20,0,0,0q�6`   $��36(�6s   $��Wyznaczanie trasy��6�   $��50,0,0,0q�6�   #   Orienteering.Views.RoutesView  5u      $��1��6$   $��	50,0,50,0q�61   5v      5w      ]� 5x      .+�5y      �  5z      �� 5{      $
��80px �6(   �� 5|      $��1* �6(   .+�5~      �� -   5        $��32��5~      $��32��6(   $
��Left=�63   $	��Top=�6N   $=��7/Orienteering;component/Resources/Images/back_arrow.png��5      $��15,5,0,0q�5�      $

 Hand��6.   6<   �� 5�      $��2��6   $(��"Resources/Fonts/seoge_ui_light.ttf)�60   $��20,0,0,0q�6`   $��36(�6s   $��Zarządzanie uczestnikami��6�   $��50,0,0,0q�6�   "   Orienteering.Views.UsersView  5�      $��1��6#   $��	50,0,50,0q�60   5�      5�      ]� 5�      .+�5�      �  5�      �� 5�      $
��80px �6(   �� 5�      $��1* �6(   .+�5�      �� -   5        $��32��5�      $��32��6(   $
��Left=�63   $	��Top=�6N   $=��7/Orienteering;component/Resources/Images/back_arrow.png��5�      $��15,5,0,0q�5�      $

 Hand��6.   6<   �� 5�      $��2��6   $(��"Resources/Fonts/seoge_ui_light.ttf)�60   $��20,0,0,0q�6`   $��36(�6s   $��Kreator zawodów��6�   $��50,0,0,0q�6�   $   Orienteering.Views.ContestView  5�      -   5          Name$ contestView��5�   %   $	   Orienteering.Views.ContestView 	  Loaded$ contestView_Loaded��6Z   $��1��6:   $��	50,0,50,0q�6G   6Z   5�      5�      5�      5�      5�      