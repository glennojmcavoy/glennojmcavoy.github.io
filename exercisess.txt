package exercises with SPARK_Mode is

   subtype Size is Integer range 0..20;
   type SmallArray is array (Size) of Integer;


   procedure Calc(Cost: in Integer; Result: out Integer)
     with
       pre => Cost <= Integer'Last/100 AND Cost >= Integer'First/100,
       post => Result = (Cost * 100) OR Result = 0;

   procedure CalculateCharge(Quantity: in Integer; Charge: out Integer)
     with
       Depends => (Charge => Quantity),
       pre => Quantity > 0 AND Quantity <= Integer'Last/5000,
       post => Charge >= 5000 AND Charge <= Integer'Last;


   procedure Modify(A: in out SmallArray; B, C: in Integer)
     with
       Depends => (A => (A,B,C)),
       pre => B >= 0 AND THEN B <= 20 AND THEN C >= 0 AND
       THEN C <= 20 AND THEN B + C >= 0 AND THEN B + C <= 20,
       post => A(B+C) = 40;

   procedure MaskSequence (Target: in out SmallArray; Mask: in SmallArray)
     with
       Depends => (Target => (Target, Mask)),
       pre => (for all L in 0 .. Mask'Last => (Mask (L) = 0 OR Mask (L) = 1)),
       post => (for all M in 0 .. Target'Last => (Target (M) = 0 OR Target (M)
                = Target'Old(M)));


end exercises;
