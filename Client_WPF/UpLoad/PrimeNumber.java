import java.util.*;

public class PrimeNumber {

	public static void main(String[] args) {
		Scanner ip = new Scanner(System.in);
		int n = ip.nextInt();
		int flag,count=0;
		for(int i=2;i<=n;i++)
		{
			flag=0;
			for(int j=2;j<=Math.sqrt(i);j++)
			{
				if(i%j==0)
					flag=1;
			}
			if(flag==0)
				count++;
		}
		System.out.println("Number of prime less then "+n+ "are "+ count);
	}

}
