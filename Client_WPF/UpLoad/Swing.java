import java.awt.event.*;
import javax.swing.*;


class GUI
{
	JFrame f;
	JLabel t,t1;
	public void action()
	{
		f = new JFrame("This is title");
		
		JButton b = new JButton("ClickMe");
		b.setBounds(100,100,150, 40);
		f.setVisible(true);
		f.setSize(400, 500);
		f.add(b);
		f.setLayout(null);
			
		t = new JLabel();
		t.setText("HI");
		t.setBounds(20, 30, 40, 20);
		
		f.add(t);
		
		b.addActionListener(new Listnr(this));
		
	}
	
	void addLabel()
	{
		t1 = new JLabel();
		t1.setText("Click");
		t1.setBounds(300, 30, 40, 20);
		t1.setVisible(true);
		f.add(t1);
	}
}

class Listnr implements ActionListener{
	
	GUI gu;
	Listnr(GUI g)
	{
		gu=g;
	}
	public void actionPerformed(ActionEvent e)
	{
		String a = e.getActionCommand();
		//Object b = e.getSource();
		if(a.equals("ClickMe"))
		{
			gu.addLabel();
		    System.out.println("Listened");
		}
	}
}

public class Swing {

	public static void main(String[] args) {
	GUI g = new GUI();
	g.action();
	}
}
