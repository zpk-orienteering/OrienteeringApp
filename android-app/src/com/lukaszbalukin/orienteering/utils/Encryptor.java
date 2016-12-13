package com.lukaszbalukin.orienteering.utils;

import java.io.UnsupportedEncodingException;
import java.security.MessageDigest;
import java.util.Arrays;

import javax.crypto.Cipher;
import javax.crypto.spec.SecretKeySpec;

import android.util.Log;

import com.lukaszbalukin.orienteering.models.OrienteeringRun;

public class Encryptor
{
	public byte[] Encrypt(String runHeader, String dataToEncrypt)
	{
		try
		{
			byte[] key = GenerateKey(runHeader);
			SecretKeySpec secretKeySpec = new SecretKeySpec(key, "AES");
			Cipher cipher = Cipher.getInstance("AES");
		    cipher.init(Cipher.ENCRYPT_MODE, secretKeySpec);

		    byte[] encrypted = cipher.doFinal((dataToEncrypt).getBytes("UTF-8"));		    
			return encrypted;			
		}
		catch(Exception ex)
		{
			Log.e("ORCrypt", "Problem z szyfrowaniem.");
			return null;
		}
	}
	
	public String Decrypt(String runHeader, byte[] dataToDecrypt)
	{
		try
		{
			byte[] key = GenerateKey(runHeader);
			SecretKeySpec secretKeySpec = new SecretKeySpec(key, "AES");
			Cipher cipher = Cipher.getInstance("AES");
		    cipher.init(Cipher.DECRYPT_MODE, secretKeySpec);

		    byte[] decrypted = cipher.doFinal(dataToDecrypt);		    
			return new String(decrypted, "UTF-8");			
		}
		catch(Exception ex)
		{
			Log.e("ORCrypt", "Problem z szyfrowaniem.");
			return null;
		}
	}
	
	private byte[] GenerateKey(String runHeader) throws Exception
	{
		byte[] key = ("SomeSecretString123" + runHeader).getBytes("UTF-8");
		MessageDigest sha = MessageDigest.getInstance("SHA-1");
		key = sha.digest(key);
		byte[] shortKey = new byte[16];
		System.arraycopy(key, 0, shortKey, 0, 16);
		return shortKey;

	}
}
