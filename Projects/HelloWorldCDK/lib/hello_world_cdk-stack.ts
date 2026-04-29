import * as cdk from 'aws-cdk-lib/core';
import { Construct } from 'constructs';
import * as sqs from 'aws-cdk-lib/aws-sqs';
import * as sns from 'aws-cdk-lib/aws-sns'
import * as sns_subscriptions from 'aws-cdk-lib/aws-sns-subscriptions'

export class HelloWorldCdkStack extends cdk.Stack {
  constructor(scope: Construct, id: string, props?: cdk.StackProps) {
    super(scope, id, props);

    const snsTopic = new sns.Topic(this, "SNS", {
      fifo: true,
    });

    const sqsA = new sqs.Queue(this, "SQS-A", {
      fifo: true,
    });

    const sqsB = new sqs.Queue(this, "SQS-B", {
      fifo: true,
    });

    snsTopic.addSubscription(new sns_subscriptions.SqsSubscription(sqsA, {
      rawMessageDelivery: true
    }));
    snsTopic.addSubscription(new sns_subscriptions.SqsSubscription(sqsB, {
      rawMessageDelivery: true
    }));
  }
}
