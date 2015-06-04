import java.util.*;

public class SumEqualToK {

	public static void main(String[] args) {
		
		Scanner sc = new Scanner(System.in);
		int a= sc.nextInt();
		int arr[]= new int[a];
		int c=0;
		for(int i=0;i<a;i++)
		{
			arr[i]=c;
			c=c+5;
		}
		int k=20;
		int p1=0,p2=a-1;
		// Maintain 2 pointers and check if sum is less than K then increase p1 or else decrease p2
		for(int i=0;i<a;i++)
		{
			if(arr[p1]+arr[p2]==k)
			{
				System.out.println("got match");
				System.out.println(arr[p1] + " "+ arr[p2]);
			}
			else if (arr[p1]+arr[p2]>k)
				p2--;
			else if (arr[p1]+arr[p2]<k)
				p1++;
			
		}
		

	}

}
