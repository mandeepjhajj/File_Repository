/*
 * This input is sorted array of 0's and 1's and we have to count the number of 0's
 */


import java.util.*;

public class CountZero {

	public static void main(String[] args) {
		Scanner s = new Scanner(System.in);
		System.out.println("enter the size of array");
		int n= s.nextInt();
		int a[]= new int[n];
		for(int i=0;i<n;i++)
		{
			a[i]=s.nextInt();
		}
		System.out.println("Entered array is: ");
		for(int b:a)
		{
			System.out.println(b);
		}
		
		CountZero z = new CountZero();
		int count=z.countZero(a,0,a.length);
		System.out.println("Number of 0's :"+count);
	}
	public int countZero(int a[],int start, int end)
	{
		int mid = Math.round((end+start)/2);
		System.out.println("Mid value is "+mid);
		
		if(start==end)
			return start;
		
		if(a[mid]==0)
		{
			return countZero(a,mid+1,end);
		}
		else
		{
			return countZero(a,start,mid);
		}		
	}

}
