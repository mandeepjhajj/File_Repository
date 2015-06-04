
	import java.awt.*;
	import java.applet.*;
	import java.awt.event.*;

	/*
	 <applet code="CalcDemo" width=300 height=300>
	 </applet>
	*/

	public class CalcDemo1 extends Applet implements ActionListener
	{
	 Label lfno,lsno,lrs;
	 TextField tfno,tsno,trs;
	 Button badd,bsub,bmul,bdiv,bclr;
	 Button b1,b2,b3,b4,b5,b6,b7,b8,b9,b0;
	 int a,b,c,m;
	 ActionEvent ae;
	 
	 public void init()
	 {
		
	 
	 	lfno = new Label("First no.");
		lsno= new Label("Second no");
		lrs = new Label("RESULT");
		tfno = new TextField();
		tsno = new TextField();
		trs = new TextField();
		
		badd =new Button("ADD");
		bsub = new Button("SUB");
		bmul = new Button("MUL");
		bdiv = new Button("DIV");
		bclr = new Button("CLEAR");

		

		setLayout(null);
		lfno.setBounds(30,30,40,20);	
		tfno.setBounds(90,30,50,20);	
		lsno.setBounds(30,55,40,20);	
		tsno.setBounds(90,55,50,20);	
		lrs.setBounds(30,80,30,20);	
		trs.setBounds(70,80,30,20);	
		badd.setBounds(30,105,30,20);	
		bsub.setBounds(70,105,30,20);	
		bmul.setBounds(30,130,30,20);	
		bdiv.setBounds(70,130,30,20);	
		bclr.setBounds(50,170,50,20);	
		
		
		add(lfno);
		add(tfno);
		add(lsno);
		add(tsno);
		add(lrs);
		add(trs);
		add(badd);
		add(bsub);
		add(bmul);
		add(bdiv);
		add(bclr);
		                          	

		badd.addActionListener(this);
		bsub.addActionListener(this);
		bmul.addActionListener(this);
		bdiv.addActionListener(this);
		bclr.addActionListener(this);


		tfno.setText(""+0);
		tsno.setText(""+0);
		trs.setText(""+0);
	 }

	 public void actionPerformed(ActionEvent ae)
	 {
		Object ob = ae.getSource();
		a=0;
		b=0;
		c=0;
		a=Integer.parseInt(tfno.getText().trim());
		b=Integer.parseInt(tsno.getText().trim());

		if(ob==badd)
		 c=a+b;
		if(ob==bsub)
		 c=a-b;
		if(ob==bmul)
		 c=a*b;
		if(ob==bdiv)
		 c=a/b;
		if(ob==bclr)
		{
		  tfno.setText(""+0);
		  tsno.setText(""+0);
		  trs.setText(""+0);
		}
	    trs.setText(""+c);
	 }

	}

