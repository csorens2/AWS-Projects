import {en, es, Faker, faker, fakerEN} from '@faker-js/faker';
import { Transaction } from "../lib/transaction";

type GenerationOrder = {
    unique_account_count: number
    transaction_count: number
}

export const handler = async (event: GenerationOrder) : Promise<string> => {

    const fakerInstance = fakerEN

    const nextTransaction: Transaction = {
        AccountName: fakerInstance.finance.accountName(),
        AccountNumber: fakerInstance.finance.accountNumber(10),
        Amount: Number(fakerInstance.finance.amount( {min: 1, max: 10}))
    }

    return 'Success'
}