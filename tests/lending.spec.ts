import { test, expect } from '@playwright/test';
import { validApplicant, invalidApplicants } from './test-data';

test.describe('Lending Application', () => {

  test.beforeEach(async ({ page }) => {
    await page.goto('/login');

    await page.getByLabel('Username').fill(validApplicant.username);
    await page.getByLabel('Password').fill(validApplicant.password);
    await page.getByRole('button', { name: 'Login' }).click();

    await expect(page).toHaveURL(/dashboard/);
  });

  test('should successfully submit a loan application', async ({ page }) => {

    await page.getByRole('link', { name: 'New Loan Application' }).click();

    await page.getByLabel('Customer Name').fill(validApplicant.customerName);
    await page.getByLabel('Annual Income').fill(validApplicant.income);
    await page.getByLabel('Credit Score').fill(validApplicant.creditScore);
    await page.getByLabel('Loan Amount').fill(validApplicant.loanAmount);

    await page.getByRole('button', { name: 'Submit Application' }).click();

    await expect(
      page.getByText('Loan application submitted successfully')
    ).toBeVisible();

    await expect(
      page.getByTestId('application-status')
    ).toHaveText('Submitted');
  });

  for (const applicant of invalidApplicants) {

    test(`should reject application - ${applicant.name}`, async ({ page }) => {

      await page.getByRole('link', { name: 'New Loan Application' }).click();

      await page.getByLabel('Customer Name').fill('Test Customer');
      await page.getByLabel('Annual Income').fill(applicant.income);
      await page.getByLabel('Credit Score').fill(applicant.creditScore);
      await page.getByLabel('Loan Amount').fill(applicant.loanAmount);

      await page.getByRole('button', { name: 'Submit Application' }).click();

      await expect(
        page.getByRole('alert')
      ).toHaveText(applicant.expectedMessage);
    });
  }

  test('should require mandatory loan application fields', async ({ page }) => {

    await page.getByRole('link', { name: 'New Loan Application' }).click();

    await page.getByRole('button', { name: 'Submit Application' }).click();

    await expect(
      page.getByText('Customer Name is required')
    ).toBeVisible();

    await expect(
      page.getByText('Annual Income is required')
    ).toBeVisible();

    await expect(
      page.getByText('Credit Score is required')
    ).toBeVisible();

    await expect(
      page.getByText('Loan Amount is required')
    ).toBeVisible();
  });

  test('should prevent loan amount above customer eligibility', async ({ page }) => {

    await page.getByRole('link', { name: 'New Loan Application' }).click();

    await page.getByLabel('Customer Name').fill('John Smith');
    await page.getByLabel('Annual Income').fill('50000');
    await page.getByLabel('Credit Score').fill('750');
    await page.getByLabel('Loan Amount').fill('200000');

    await page.getByRole('button', { name: 'Submit Application' }).click();

    await expect(
      page.getByRole('alert')
    ).toBeVisible();
  });

});