import java.util.*;


public class InsertionSort {

	public static void main(String[] args) {
		System.out.println("Enter the size of array for sorting ");
		Scanner sc = new Scanner(System.in);
		int size= sc.nextInt();
		int list[]= new int[size];
		 for(int i=0;i<size;i++)
		 {
			 list[i]=sc.nextInt();
		 }
		 System.out.println("Entered array is");
		 for(int i=0;i<size;i++)
		 {
			 System.out.print("  "+list[i]);
		 }
		 
		 // Alternate //
		 /*int pos=0,val=0;
		 for(int i=0;i<list.length;i++)
		 {
			pos=i;
			val=list[i];
			
			while(pos>=1 && list[pos-1]>val)
			{
				list[pos]=list[pos-1];
				pos--;
			}
			list[pos]=val;
		 }*/
		 
		 for(int i=1;i<list.length;i++)
		 {
			 int key = list[i];
			 int j=i-1;
			 
			 while(j>=0 && list[j]>key)
			 {
				 list[j+1]=list[j];
				 j=j-1;
			 }
			 list[j+1]=key;
		 }
		 System.out.println("\nSoreted array is :");
		 for(int i=0;i<size;i++)
		 {
			 System.out.print("  "+list[i]);
		 }
	}
}
