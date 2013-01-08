////////////////////////////////////////////////////////////////////////////////
//
// CRYSTAL CLEAR SOFTWARE
// Copyright 2012 Crystal Clear Software. http://ccsoft.ru
// All Rights Reserved. Unity CCSoft lib
// @author Osipov Stanislav lacost.20@gmail.com
//
//
// NOTICE: Crystal Soft does not allow to use, modify, or distribute this file
// for any purpose
//
////////////////////////////////////////////////////////////////////////////////

public class sApiUserInfo 
{
	public string  id		 = "";//ID в соцсети
	public string name 		 = "empty";//Имя
	public string datName; 	// Имя пользователя в дательном падеже
	public string genName; // Имя пользователя в родительном падеже
	public string fullName 	= "empty";//ФИО
	public string city = "empty";//Родной город
	public string birthDate = "empty"; // probably date format
	public string url = "empty";//URL страницы пользователя
	public string photo = "empty";//URL аватарки
	public bool	  isAppUser = false;//Установил ли приложение пользователь
	public int	  age	= 0; // возраст
	public SEX 	  sex	=SEX.UNDEFINED; // пол
}

