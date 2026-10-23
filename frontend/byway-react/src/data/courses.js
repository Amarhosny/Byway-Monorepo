export const courses = Array.from({ length: 9 }).map((_, i) => ({
  id: i + 1,
  title: "Beginner's Guide to Design",
  category: 'UI/UX Design',
  instructor: 'Ronald Richards',
  rating: 5,
  hours: 22,
  lectures: 155,
  level: 'Beginner',
  price: 45,
}));

export const categories = [
  { name: 'Fullstack', count: 11, icon: '📣' },
  { name: 'Backend', count: 12, icon: '🗄️' },
  { name: 'Frontend', count: 12, icon: '💼' },
  { name: 'UX/UI Design', count: 14, icon: '⌗' },
];

export const testimonials = [
  {
    name: 'Jane Doe',
    role: 'Designer',
    quote:
      "Byway's tech courses are top-notch! As someone who's always looking to stay ahead in the rapidly evolving tech world, I appreciate the up-to-date content and engaging multimedia.",
  },
  {
    name: 'Jane Doe',
    role: 'Designer',
    quote:
      "Byway's tech courses are top-notch! As someone who's always looking to stay ahead in the rapidly evolving tech world, I appreciate the up-to-date content and engaging multimedia.",
  },
  {
    name: 'Jane Doe',
    role: 'Designer',
    quote:
      "Byway's tech courses are top-notch! As someone who's always looking to stay ahead in the rapidly evolving tech world, I appreciate the up-to-date content and engaging multimedia.",
  },
];
