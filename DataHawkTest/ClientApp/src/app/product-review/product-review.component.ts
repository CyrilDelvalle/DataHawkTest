import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-product-review',
  templateUrl: './product-review.component.html'
})
export class ProductReviewComponent {
  public customerReviews: CustomerReview[];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<CustomerReview[]>(baseUrl + 'CustomerReview').subscribe(result => {
      this.customerReviews = result;
      console.log(this.customerReviews);
    }, error => console.error(error));
  }
}

interface CustomerReview {
  AmazonASIN: string;
  ReviewId: string;
  ReviewAuthor: string;
  ReviewTitle: string;
  ReviewRate: number;
  ReviewBody: string;
  ReviewDate: string;
}
