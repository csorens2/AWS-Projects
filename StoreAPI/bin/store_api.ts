import * as cdk from 'aws-cdk-lib/core';
import {StoreApiStack} from "../lib/store_api-stack"

const app = new cdk.App();

const storeStack = new StoreApiStack(app, 'StoreStack')