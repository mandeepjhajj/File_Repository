
import java.util.*;
import java.lang.*;
import java.io.*;

/* Name of the class has to be "Main" only if the class is public. */
class UnCommonN
{
	
	public static void findUnCommonNumbers(int[] a,int[] b)
	{
		HashMap<Integer,Integer> input = new HashMap<Integer,Integer>();
		
		for(int i:a)
		{
			input.put(i,i);
		}
		
		for(int j:b)
		{
			if(!input.containsKey(j))
			{
				System.out.print(" "+j);
			}
		}
	}
	
	public static void main (String[] args) throws java.lang.Exception
	{
		// your code goes here
		int a[] ={1, 4, 2, 6, 3} ;
		int b[]={4, 0,7, 6, 3, 2, 1};
		
		
		findUnCommonNumbers(a,b);
	}
}