import java.util.*;

public class APMisingTerm {
	public static void main(String[] args) {
		Scanner ip = new Scanner(System.in);
		int num = ip.nextInt();
		
		int ap[] = new int[num];
		int diff[]= new int[num-1];
		for(int i=0;i<num;i++)
		{
			ap[i]=ip.nextInt();
		}
		System.out.println("Entered array is: ");
		for(int a:ap)
		{
			System.out.println(a);
		}
		
		System.out.println("Array of differences : ");
		for(int i=0;i<num-1;i++)
		{
			diff[i]=ap[i+1]-ap[i];
			System.out.println(diff[i]);	
		}
		int mis;
		System.out.println("logic : ");
		
		for(int i=0;i<diff.length-1;i++)
		{
			if(diff[i]!=diff[i+1])
			{
				mis=i+1;
				System.out.println("index"+mis);
				int d=ap[mis]+diff[i];
				System.out.println("Missing term is :"+d);
				break;
			}
			
		}
	}

}
