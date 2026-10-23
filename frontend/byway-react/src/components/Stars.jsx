export default function Stars({ rating = 5, outOf = 5 }) {
  return (
    <div className="stars">
      {'★'.repeat(rating)}
      {'☆'.repeat(Math.max(0, outOf - rating))}
    </div>
  );
}
