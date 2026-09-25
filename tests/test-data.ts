export const validApplicant = {
  username: 'loan.officer',
  password: 'Password@123',
  customerName: 'John Smith',
  income: '75000',
  creditScore: '720',
  loanAmount: '25000'
};

export const invalidApplicants = [
  {
    name: 'Low Credit Score',
    income: '75000',
    creditScore: '580',
    loanAmount: '25000',
    expectedMessage: 'Credit score is below the minimum requirement'
  },
  {
    name: 'High Loan Amount',
    income: '75000',
    creditScore: '720',
    loanAmount: '500000',
    expectedMessage: 'Loan amount exceeds the permitted limit'
  },
  {
    name: 'Zero Income',
    income: '0',
    creditScore: '720',
    loanAmount: '25000',
    expectedMessage: 'Income must be greater than zero'
  }
];