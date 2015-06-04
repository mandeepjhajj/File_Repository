package Zillow;

import java.lang.*;

public class StringToLong {

	/**
	 * @param args
	 */

	public static long parseLongJava(String s, int radix)
	               throws NumberFormatException
	     {
	         if (s == null) {
	             throw new NumberFormatException("null");
	         }
	 
	         if (radix < Character.MIN_RADIX) {
	             throw new NumberFormatException("radix " + radix +
	                                             " less than Character.MIN_RADIX");
	         }
	         if (radix > Character.MAX_RADIX) {
	           throw new NumberFormatException("radix " + radix +
	                                             " greater than Character.MAX_RADIX");
	         }
	 
	         long result = 0;
	         boolean negative = false;
	         int i = 0, len = s.length();
	         long limit = -Long.MAX_VALUE;
	         long multmin;
	         int digit;
	 
	         if (len > 0) {
	            char firstChar = s.charAt(0);
	             if (firstChar < '0') { // Possible leading "-"
	                 if (firstChar == '-') {
	                     negative = true;
	                     limit = Long.MIN_VALUE;
	                 } else
	                     throw new NumberFormatException(null);
	 
	                 if (len == 1) // Cannot have lone "-"
	                     throw new NumberFormatException(null);
	                 i++;
	             }
	             multmin = limit / radix;
	             while (i < len) {
	                 // Accumulating negatively avoids surprises near MAX_VALUE
	                 digit = Character.digit(s.charAt(i++),radix);
	                 if (digit < 0) {
	                     throw new NumberFormatException(null);
	                 }
	                 if (result < multmin) {
	                     throw new NumberFormatException(null);
	                 }
	                 result *= radix;
	                 if (result < limit + digit) {
	                     throw new NumberFormatException(null);
	                 }
	                 result -= digit;
	             }
	         } else {
	             throw new NumberFormatException(null);
	         }
	         return negative ? result : -result;
	     }
	
	
	public static void main(String[] args) {
		// TODO Auto-generated method stub

		String s = "-12345";

	     /* try {
	         long l = StringToLong.parseLongJava(s,10);
	         System.out.println("long l = " + l);
	      } catch (NumberFormatException nfe) {
	         System.out.println("NumberFormatException: " + nfe.getMessage());
	      }*/
	     /* 
	      try {
		         long l = StringToLong.covert(s);
		         System.out.println("long l = " + l);
		      } catch (NumberFormatException nfe) {
		         System.out.println("NumberFormatException: " + nfe.getMessage());
		      }
		*/
		test1();
		test2();
		test3();
		test4();
		test5();
		test6();
		test7();
		
	}
	private static void test1() {
		Long num1 = (long) (Math.random() * Long.MAX_VALUE);
		Long num2 = -(long) (Math.random() * Long.MAX_VALUE);
		StringToLong sTL = new StringToLong();
		if (num1 == sTL.covert(num1 + "") && num2 == sTL.covert(num2 + ""))
			System.out.println("Test Case 1 passed!");
		else 
			System.out.println("Test Case 1 failed...");
	}
	
	//test empty string
	private static void test2() {
		StringToLong sTL = new StringToLong();
		if (sTL.covert("") == 0)
			System.out.println("Test Case 2 passed!");
		else 
			System.out.println("Test Case 2 failed...");
	}
	
	//test multiple spaces with "+/-" sign
	private static void test3() {
		StringToLong sTL = new StringToLong();
		String num1 = "09286539937";
		String num2 = "    9377498";
		String num3 = "      +8274";
		String num4 = "           -27668836789";
		if (sTL.covert(num1) == 9286539937L && sTL.covert(num2) == 9377498
				&& sTL.covert(num3) == 8274 && sTL.covert(num4) == -27668836789L)
			System.out.println("Test Case 3 passed!");
		else 
			System.out.println("Test Case 3 failed...");
	}
	
	//test invalid character
	private static void test4() {
		StringToLong sTL = new StringToLong();
		String num1 = "00000978741       ";
		String num2 = "      -00000234564abnckdsuoe i237497284";
		if (sTL.covert(num1) == 978741 && sTL.covert(num2) == -234564)
			System.out.println("Test Case 4 passed!");
		else 
			System.out.println("Test Case 4 failed...");
	}
	
	//test Long.MAX_VALUE and Long.MIN_VALUE
	private static void test5() {
		StringToLong sTL = new StringToLong();
		String num1 = Long.MAX_VALUE + "";
		String num2 = Long.MIN_VALUE + "";
		if (sTL.covert(num1) == Long.MAX_VALUE && sTL.covert(num2) == Long.MIN_VALUE)
			System.out.println("Test Case 5 passed!");
		else 
			System.out.println("Test Case 5 failed...");
	}
	
	//test overflow
	private static void test6() {
		StringToLong sTL = new StringToLong();
		String num1 = "9223372036854775808";
		String num2 = "923840981238974365921734";
		String num3 = "-9223372036854775808";
		String num4 = "-209384843578320938423890482304823";
		if (sTL.covert(num1) == Long.MAX_VALUE && sTL.covert(num2) == Long.MAX_VALUE
				&& sTL.covert(num3) == Long.MIN_VALUE && sTL.covert(num4) == Long.MIN_VALUE)
			System.out.println("Test Case 6 passed!");
		else 
			System.out.println("Test Case 6 failed...");
	}
	
	//test invalid input
	private static void test7() {
		StringToLong sTL = new StringToLong();
		String num1 = "     ";
		String num2 = "+";
		String num3 = "   a   ";
		String num4 = "anclajsdfkl";
		if (sTL.covert(num1) == 0 && sTL.covert(num2) == 0 &&
				sTL.covert(num3) == 0 && sTL.covert(num4) == 0)
			System.out.println("Test Case 7 passed!");
		else 
			System.out.println("Test Case 7 failed...");
	}

	
	public long covert(String s) {
		if (s == null)
			return 0;
		int len = s.length();
		int start = 0;
		boolean isNeg = false;
		long res = 0;
		//empty string
		if (len == 0)
			return 0;
		//skip white spaces
		while (start < len && s.charAt(start) == ' ')
			start++;
		//if reach end, return 0
		if (start == len)
			return 0;
		//deal with optional sign '+' and '-'
		if (s.charAt(start) == '+' || s.charAt(start) == '-') {
			isNeg = s.charAt(start) == '-'? true: false;
			start++;
		}
		//if reach end, return 0
		if (start == len)
			return 0;
		for (int i = start; i < len; i++) {
			//if encounter invalid character
			if (!isNum(s.charAt(i)))
				return res;
			int add = s.charAt(i) - '0';
			//test overflow
			if (isOverflow(isNeg, res, add))
				return isNeg? Long.MIN_VALUE: Long.MAX_VALUE;
			else 
				res = isNeg? res * 10 - add: res * 10 + add;
		}
		return res;
	}
	
	private boolean isOverflow(boolean isNeg, long base, int add) {
		if (!isNeg) {
			long upperBound = (Long.MAX_VALUE - add) / 10;
			return base > upperBound;
		} else {
			long lowerBound = (Long.MIN_VALUE + add) / 10;
			return base < lowerBound;
		}
	}
	
	private boolean isNum(char c) {
		return c - '0' <= 9 && c - '0' >= 0;
	}
	
	
	
	
}
