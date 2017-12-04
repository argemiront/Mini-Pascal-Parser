int main()
{
int X;
int I;
bool B;

int t1 = 1;
int t2 = t1;
int t3 = t2;
I = t3;
L2:
int t4 = I;
int t5 = t4;
int t6 = 10;
int t7 = t6;
int t8 = t5<=t7;
if (!t8) goto L1;
printf("", 'N');
scanf("%c", &X);
printf("%i", I);
scanf("%c", &X);
int t9 = 1;
int t10 = I;
int t11 = t10+t9;
int t12 = t11;
I = t12;
goto L2;
L1:
int t13 = 1;
int t14 = t13;
int t15 = t14;
I = t15;
L4:
int t16= !B;
int t17 = t16;
int t18 = t17;
int t19 = t18;
if (!t19) goto L3;
int t20 = X;
int t21 = t20;
int t22 =  'C';
int t23 = t22;
int t24 = t21==t23;
if (!t24) goto L6;
int t25 = true;
int t26 = t25;
int t27 = t26;
B = t27;
printf("", 'C');
goto L5;
L6:
int t28 = I;
int t29 = t28;
int t30 = 10;
int t31 = t30;
int t32 = t29<t31;
if (!t32) goto L8;
int t33 = 1;
int t34 = I;
int t35 = t34+t33;
int t36 = t35;
I = t36;
goto L7;
L8:
int t37 = true;
int t38 = t37;
int t39 = t38;
B = t39;
goto L7;
L7:
goto L5;
L5:
goto L4;
L3:
system("pause");
return 0;
}
