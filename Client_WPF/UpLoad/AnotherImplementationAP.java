import java.util.*;

public class AnotherImplementationAP {

	public static void main(String[] args) {
	
		Scanner sc = new Scanner(System.in);
		int num = sc.nextInt();
		
		int a[]= new int[num];
		for(int i=0;i<num;i++)
		{
			a[i]=sc.nextInt();
		}
		
		System.out.println("Entered array is: ");
		for(int b:a)
		{
			System.out.println(b);
		}
		
		//calculate the differece d
		int difference=0;
	
			int a1=a[1]-a[0];
			int a2=a[2]-a[1];
			int a3=a[3]-a[2];
			
			if(a1==a2 || a1==a3)
				difference=a1;
			if(a2==a3 || a2==a1)
				difference=a2;
	
		MissingElements(difference,a,num);
	}

	public static void MissingElements(int d, int i[],int n)
	{
		int last= i[n-1],flag;
		int nextNumber = i[0]+d;
		System.out.println(last);
		System.out.println(nextNumber);
		while(nextNumber!=last)
		{
			flag=0;
			for(int j=0;j<n;j++)
			{
				if(nextNumber==i[j])
				{
					flag=1;
					System.out.println("Nest number is "+nextNumber);
				}
			}
			if(flag==0)
			{
				System.out.println("Missing number is "+nextNumber);
				break;
			}
				nextNumber=nextNumber+d;
		}
	}
}
