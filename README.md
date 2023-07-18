Developer Code Exam

Provide a software solution to model the operation of a vending machine, including a GUI (i.e.not a console application), using desktop or web technologies, able to be run on a standard Microsoft Windows computerand/or browser.

All requirements must be implemented. Any assumptions should be stated. You should write production quality and well documented code along with appropriate tests.

The vending machine can stock up to 10 different flavours of cans, but they are currently all the same price. The value of cans may be decided by you at design time, but should cater for dollars and cents values.

The vending machine can take payment either through cash or credit card, with the expectation that card and cash payments are made with the correct change.

The vending machine can eject one can per payment. Assume that there can’t be any errors or faults with ejecting a can.

The vending machine will need to track the number of cans available, the money (in dollars/cents value) currently held in the machine, and the amount (in dollars/cents value) of credit card payments made. These should be visually displayed in the GUI.

This information can be held in memory during runtime and does not necessarily need to be persisted.

Restocking the machine assumes that:
  *the amount of cash held in the machine has been reset to zero
  *the amount of credit card payments has been reset to zero
  *the number of cans sold has been reset to zero
  *the amount of cans added during the restock is added to the available cans
