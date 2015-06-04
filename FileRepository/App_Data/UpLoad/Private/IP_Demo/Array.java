import java.util.Arrays;

public class Array {

	public static void main(String[] args) {
		int[] a= new int[10];
		int b[]= new int[5];
		
		int f[][] = new int[3][3];
		int c=0,d=0,e=0;
		for(int i=0;i<10;i++)
		{
			a[i]=c;
			c=c+5;
		}
		
		
		for(int i=0;i<5;i++)
		{
			b[i]=d;
			d=d+3;
		}
		
		for(int i=0;i<3;i++)
		{
			for(int j=0;j<3;j++)
			{
				f[i][j]=e;
				e=e+2;
			}
		}
		
		for(int i=0;i<10;i++)
		{
			System.out.println(a[i]);
		}
		
		for(int i=0;i<5;i++)
		{
			System.out.println(b[i]);
		}
		for(int i=0;i<3;i++)
		{
			for(int j=0;j<3;j++)
			{
				System.out.println(f[i][j]);
			}
		}
		
	}

}
