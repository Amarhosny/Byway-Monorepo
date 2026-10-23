# Byway — React Front-End

A front-end-only React implementation of the Byway e-learning platform,
built from the internship Figma design. No backend — all data is local
mock data, and cart/auth state live in React Context for the session only.

## Stack
- React 18 + Vite
- React Router v6 (client-side routing)
- Plain CSS (design tokens in `src/index.css`) — no UI framework

## Getting started
```bash
npm install
npm run dev
```
Then open the local URL Vite prints (usually http://localhost:5173).

To build for production:
```bash
npm run build
npm run preview
```

## Project structure
```
src/
  components/       Header, Footer, Modal, Stars, layout shells
  context/          CartContext (cart items + totals), AuthContext (login state)
  data/             Mock courses & instructors
  pages/            Public site pages (landing, auth, courses, cart, checkout...)
  pages/admin/      Admin dashboard pages (dashboard, instructors, courses)
```

## Routes
| Path                  | Page                          |
|------------------------|-------------------------------|
| `/`                    | Landing page                  |
| `/login`, `/signup`    | Auth pages                    |
| `/courses`             | Course catalog + filters      |
| `/courses/:id`         | Course details (tabs, add to cart) |
| `/cart`                | Shopping cart                 |
| `/checkout`            | Checkout form                 |
| `/purchase-complete`   | Order confirmation            |
| `/admin`               | Admin dashboard (stats)       |
| `/admin/instructors`   | Instructor management (CRUD modals) |
| `/admin/courses`       | Course management (2-step add flow) |

## Notes
- Images are CSS gradient/color placeholders standing in for the real
  photography in the Figma file — drop real assets into `src/assets` and
  swap them into the relevant components when ready.
- Login/Signup don't validate or persist anything; submitting either form
  just sets a mock "logged in" state via `AuthContext`.
- Admin CRUD actions (add/edit/delete) open the correct modals and forms
  but don't persist changes, since there's no backend — wire up API calls
  in `context/` or a new `services/` folder when you add one.
