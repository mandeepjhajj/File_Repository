//////////////////////////////////////////////////////////////////////////////
// StringConversion.java  -Convert the string to long data type.            //
// 							                                                //
// Version 1.0                                                              //
// Language:  Java						        					        //
// Platform:  HP Pavilion dv6 , Win 7, SP 1                          	    //
// Author:    Mandeep Singh, Syracuse University                            //
//            315-751-3413, mjhajj@syr.edu or mandeepsinghjhajj1@gmail.com  //
//////////////////////////////////////////////////////////////////////////////
/*
 * Module Operations
 * =================
 * This module define the following classes
 *   StringConversion - Responsible for converting string to long type
 * 
 * Public Interface
 * ================
 * 
 * public long stringToLong(String s)
 *
 * Maintenance History
 * ===================
 * 
 * Version 1.0 : 01 April 2015
 * - First release
 * 
 */


public class StringConversion {

	/*Return the digit if present*/
	private int getValue(char c){
		if(c - '0' > -1 && c - '0' < 10){
			return c-'0';
		}
		return -1;
	}
	
	/* Check if the current value of result is in range of long or not */
	private boolean inLongRange(long result, int value, boolean isNegative){
	
		if(isNegative){
			long lowLimit=(Long.MIN_VALUE+value)/10;
			return result>=lowLimit;
		}
		else{
			long upLimit =(Long.MAX_VALUE-value)/10;
			return result<upLimit;
		}
	}
	
	public long stringToLong(String s){
		
		if(s==null){
			throw new NumberFormatException(null);
		}
		long result=0;
		int start=0,end=s.length();
		int value;
		boolean isNegative=false;
		
		if(end==0){
			throw new NumberFormatException("Empty String");
		}
		/* Move pointer to location where we have first character if it's present */ 
		for(int i=start;i<end && s.charAt(start)==' ';i++){
			if(s.charAt(i)==' '){
				start++;
			}
		}
				
		/* String has no characters in it */
		if(start==end){
			throw new NumberFormatException("String has no characters in it..");
		}
		
		/* Check if string has - or + in start to determine signature */
		if(s.charAt(start)=='-' || s.charAt(start)=='+'){
			isNegative= s.charAt(start)=='-'? true:false;
			start++;
		}
		
		/* If string contains only '-' or '+' sign then throw exception */
		if(end==1 && (s.charAt(start-1)=='-'|| s.charAt(start-1)=='+')){
			throw new NumberFormatException("Your string contains only - or + symbol!!");
		}
		
		/* Convert the string to long logic*/
		while(start<end){
			value = getValue(s.charAt(start));
			if(value<0){
				long temp = isNegative? result:-result;
				throw new NumberFormatException("String contains non numeric characters, your long till this point is :"+ temp);
			}
			
			if(!inLongRange(result, value, isNegative)){
				throw new NumberFormatException("String not in long range");
			}
			else{
				result = result * 10 - value;
			}
			start++;
		}
		return isNegative? result:-result;
	}
	
	/* Test case */
	private static void test() {
		String s= "-1234";
		StringConversion convert = new StringConversion();
		long mylong=convert.stringToLong(s);
				
		if(mylong==-1234){
			System.out.println("Sucess: My long :"+mylong);
		}
		else{
			System.out.println("Failure");
		}
	}
	
	public static void main(String[] args) {
		// TODO Auto-generated method stub
		try{
		test();
		}
		catch(Exception e){
			System.out.println("String format Exception : "+e.getMessage());
		}
	}
}
