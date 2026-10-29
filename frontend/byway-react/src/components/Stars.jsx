export default function Stars({ rating = 5, outOf = 5 }) {
  const filled = Math.max(0, Math.min(outOf, Math.round(Number(rating) || 0)));
  return (
    <div className="stars" aria-label={`${rating} out of ${outOf} stars`}>
      {'★'.repeat(filled)}
      {'☆'.repeat(Math.max(0, outOf - filled))}
    </div>
  );
}
