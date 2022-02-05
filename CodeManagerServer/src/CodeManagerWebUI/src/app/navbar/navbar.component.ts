import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Select } from '@ngxs/store';
import { AppState } from '../store/app/app.state';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss'],
})
export class NavbarComponent implements OnInit {
  @Select(AppState.isAdmin) isAdmin$?: Observable<boolean>;

  constructor(private readonly router: Router) {}

  ngOnInit(): void {
    this.isAdmin$?.subscribe({
      next: (value: boolean) => {
        console.log(value);
      },
    });
  }

  async navigateTo(url: string) {
    await this.router.navigate([url]);
  }
}
